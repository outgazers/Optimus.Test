using System.Text;
using System.Text.Json.Serialization;
using Convey;
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
using Optimus.Services.Customers.Application;
using Optimus.Services.Customers.Application.Commands;
using Optimus.Services.Customers.Application.Events.External;
using Optimus.Services.Customers.Application.Services;
using Optimus.Services.Customers.Application.Storage;
using Optimus.Services.Customers.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Open.Serialization.Json.Utf8Json;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Core.Services;
using Optimus.Services.Customers.Infrastructure.Contexts;
using Optimus.Services.Customers.Infrastructure.Decorators;
using Optimus.Services.Customers.Infrastructure.Exceptions;
using Optimus.Services.Customers.Infrastructure.Postgres;
using Optimus.Services.Customers.Infrastructure.Postgres.MockData;
using Optimus.Services.Customers.Infrastructure.Postgres.Repositories;
using Optimus.Services.Customers.Infrastructure.Services;
using Optimus.Services.Customers.Infrastructure.Services.Minio;
using Optimus.Services.Customers.Infrastructure.Storage;
using SharedAbstractions.Queries;

namespace Optimus.Services.Customers.Infrastructure;

public static class Extensions
{
    public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
    {
        builder.Services
            .AddScoped<MockData>()
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<IEventMapper, EventMapper>()
            .AddTransient<IMessageBroker, MessageBroker>()
            .AddScoped<ICustomerStorage, CustomerStorage>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IVipPolicy, VipPolicy>()
            .AddTransient<IAppContextFactory, AppContextFactory>()
            .AddTransient(ctx => ctx.GetRequiredService<IAppContextFactory>().Create())
            .AddPostgres<CustomersDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddMinio()
            .AddJsonSerializer()
            .AddMvcCore()
            .AddApiExplorer()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        
        builder.Services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Customer Service",
                Version = "v1"
            });
        });
        
        builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));
        
        return builder
            .AddHttpClient()
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddRabbitMq()
            .AddMessageOutbox(o => o.AddEntityFramework<CustomersDbContext>())
            .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
            .AddRedis()
            //.AddMetrics()
            //.AddJaeger()
            .AddHandlersLogging();
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseErrorHandler()
            .UseConvey()
            .UsePublicContracts<ContractAttribute>()
            //.UseMetrics()
            .UseSwagger()
            .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"))
            .UseRabbitMq()
            .SubscribeCommand<CompleteCustomerRegistration>()
            .SubscribeCommand<ChangeCustomerState>()
            .SubscribeEvent<SignedUp>();

        RunMigration(app.ApplicationServices);
        //InitializeMockData(app.ApplicationServices);


        return app;
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

    public static IServiceCollection AddMinio(this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        var configuration = provider.GetService<IConfiguration>();
        services.Configure<MinioOptions>(configuration.GetSection("minio"));

        services.AddScoped<IFileManager, MinioFileManager>();

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

    private static void RunMigration(IServiceProvider serviceProvider)
    {
        using var scoped = serviceProvider.CreateScope();
        var context = scoped.ServiceProvider.GetRequiredService<CustomersDbContext>();
        context.Database.Migrate();
    }
    
    private static void InitializeMockData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var mockData = scope.ServiceProvider.GetRequiredService<MockData>();
        mockData.Initialize().GetAwaiter().GetResult();
    }
}
