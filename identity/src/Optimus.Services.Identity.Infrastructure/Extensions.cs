using System.Text;
using System.Text.Json.Serialization;
using Convey;
using Convey.Auth;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.HTTP;
using Convey.MessageBrokers;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.EntityFramework;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Persistence.Redis;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Optimus.Services.Identity.Application;
using Optimus.Services.Identity.Application.Commands;
using Optimus.Services.Identity.Application.Services;
using Optimus.Services.Identity.Application.Services.Auth;
using Optimus.Services.Identity.Application.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Open.Serialization.Json.System;
using Optimus.Services.Identity.Core.Repositories;
using Optimus.Services.Identity.Infrastructure.Auth;
using Optimus.Services.Identity.Infrastructure.Contexts;
using Optimus.Services.Identity.Infrastructure.Decorators;
using Optimus.Services.Identity.Infrastructure.Exceptions;
using Optimus.Services.Identity.Infrastructure.Postgres;
using Optimus.Services.Identity.Infrastructure.Postgres.Decorators;
using Optimus.Services.Identity.Infrastructure.Postgres.Repositories;
using Optimus.Services.Identity.Infrastructure.Services;
using SharedAbstractions.Queries;

namespace Optimus.Services.Identity.Infrastructure;

public static class Extensions
{
    public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
    {
        builder.Services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<IDispatcher, InMemoryDispatcher>()
            .AddSingleton<IJwtProvider, JwtProvider>()
            .AddSingleton<IPasswordService, PasswordService>()
            .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>()
            .AddSingleton<IHashHelper, HashHelper>()
            .AddTransient<IIdentityService, IdentityService>()
            .AddTransient<IRefreshTokenService, RefreshTokenService>()
            .AddSingleton<IRng, Rng>()
            .AddSingleton<IEmailService, EmailService>()
            .AddTransient<IMessageBroker, MessageBroker>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ICrmService, CrmService>()
            .AddTransient<IAppContextFactory, AppContextFactory>()
            .AddTransient(ctx => ctx.GetRequiredService<IAppContextFactory>().Create())
            .AddPostgres<IdentityDbContext>()
            .AddJsonSerializer()
            .AddMvcCore()
            .AddApiExplorer()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "Identity Service", Version = "v1"});
        });
        
        builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));
        builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandlerDecorator<>));
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(TransactionalEventHandlerDecorator<>));

        return builder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddJwt()
            .AddHttpClient()
            .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
            .AddRabbitMq()
            .AddMessageOutbox(o => o.AddEntityFramework<IdentityDbContext>())
            .AddRedis();
        //.AddMetrics()
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseErrorHandler()
            .UseConvey()
            .UseAccessTokenValidator()
            .UsePublicContracts<ContractAttribute>()
            //.UseMetrics()
            .UseAuthentication()
            .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"))
            .UseSwagger()
            .UseRabbitMq()
            .SubscribeCommand<SignUp>();

        RunMigration(app.ApplicationServices);

        return app;
    }

    public static async Task<Guid> AuthenticateUsingJwtAsync(this HttpContext context)
    {
        var authentication = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

        return authentication.Succeeded ? Guid.Parse(authentication.Principal.Identity.Name) : Guid.Empty;
    }

    internal static CorrelationContext GetCorrelationContext(this IHttpContextAccessor accessor)
        => accessor.HttpContext?.Request.Headers.TryGetValue("Correlation-Context", out var json) is true
            ? JsonConvert.DeserializeObject<CorrelationContext>(json.FirstOrDefault())
            : null;
    
    internal static IDictionary<string, object> GetHeadersToForward(this IMessageProperties messageProperties)
    {
        const string sagaHeader = "Saga";
        if (messageProperties?.Headers is null || !messageProperties.Headers.TryGetValue(sagaHeader, out var saga))
        {
            return null;
        }

        return saga is null
            ? null
            : new Dictionary<string, object>
            {
                [sagaHeader] = saga
            };
    }
    
    internal static string GetSpanContext(this IMessageProperties messageProperties, string header)
    {
        if (messageProperties is null)
        {
            return string.Empty;
        }

        if (messageProperties.Headers.TryGetValue(header, out var span) && span is byte[] spanBytes)
        {
            return Encoding.UTF8.GetString(spanBytes);
        }

        return string.Empty;
    }
    
    public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }
    
    public static IServiceCollection AddPostgres<T>(this IServiceCollection services) where T : DbContext
    {
        var options = services.GetOptions<PostgresOptions>("postgres");
        
        services.AddDbContext<T>(x => x.UseNpgsql(options.ConnectionString));

        return services;
    }
    
    public static Task<Paged<T>> PaginateAsync<T>(this IQueryable<T> data, IPagedQuery query,
        CancellationToken cancellationToken = default)
        => data.PaginateAsync(query.Page, query.Results, cancellationToken);

    public static async Task<Paged<T>> PaginateAsync<T>(this IQueryable<T> data, int page, int results,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0)
        {
            page = 1;
        }

        results = results switch
        {
            <= 0 => 10,
            > 100 => 100,
            _ => results
        };

        var totalResults = await data.CountAsync();
        var totalPages = totalResults <= results ? 1 : (int) Math.Floor((double) totalResults / results);
        var result = await data.Skip((page - 1) * results).Take(results).ToListAsync(cancellationToken);

        return new Paged<T>(result, page, results, totalPages, totalResults);
    }
    
    public static Task<List<T>> SkipAndTakeAsync<T>(this IQueryable<T> data, IPagedQuery query,
        CancellationToken cancellationToken = default)
        => data.SkipAndTakeAsync(query.Page, query.Results, cancellationToken);
    
    public static async Task<List<T>> SkipAndTakeAsync<T>(this IQueryable<T> data, int page, int results,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0)
        {
            page = 1;
        }

        results = results switch
        {
            <= 0 => 10,
            > 100 => 100,
            _ => results
        };

        return await data.Skip((page - 1) * results).Take(results).ToListAsync(cancellationToken);
    }
    
    private static void RunMigration(IServiceProvider serviceProvider)
    {
        using var scoped = serviceProvider.CreateScope();
        var context = scoped.ServiceProvider.GetRequiredService<IdentityDbContext>();
        context.Database.Migrate();
    }
}