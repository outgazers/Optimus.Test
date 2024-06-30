using Convey;
using Convey.Logging;
using Convey.Metrics.AppMetrics;
using Exchange.APIGateway.Infrastructure;
using Exchange.ApiGateway.Infrastructure.Decorators;
using Exchange.APIGateway.Infrastructure.Middlewares;
using Ntrada;
using Ntrada.Extensions.RabbitMq;
using Ntrada.Hooks;

var builder = WebApplication.CreateBuilder(args);

const string extension = "yml";
var ntradaConfig = Environment.GetEnvironmentVariable("NTRADA_CONFIG");
var configPath = args?.FirstOrDefault() ?? ntradaConfig ?? $"ntrada.{extension}";
if (!configPath.EndsWith($".{extension}"))
{
    configPath += $".{extension}";
}

builder.Configuration.AddYamlFile(configPath, false);

builder.Services.AddNtrada()
    .AddSingleton<IContextBuilder, CorrelationContextBuilder>()
    .AddSingleton<ISpanContextBuilder, SpanContextBuilder>()
    .AddSingleton<IHttpRequestHook, HttpRequestHook>()
    .AddSingleton<DownstreamHandlerDecorator>()
    .AddConvey();
    //.AddMetrics();

builder.Services.TryDecorate<IAuthorizationManager, AuthorizationManagerDecorator>();
builder.Services.TryDecorate<IRequestHandlerManager, RequestHandlerManagerDecorator>();

builder.WebHost.UseLogging();

var app = builder.Build();

app.RegisterRequestHandlerManager()
    .UseNtrada();

app.Run();