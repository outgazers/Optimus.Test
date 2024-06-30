using Convey;
using Convey.Logging;
using Optimus.Services.Identity.Application;
using Optimus.Services.Identity.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConvey()
    .AddApplication()
    .AddInfrastructure();

// Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.With()
    .CreateLogger();
Log.Error($"Environment : {builder.Environment}");

builder.WebHost.UseLogging();

var app = builder.Build();

app.UseInfrastructure()
    .UseRouting()
    .UseEndpoints(e => e.MapControllers());

app.Run();