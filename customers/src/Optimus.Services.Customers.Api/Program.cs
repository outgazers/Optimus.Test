using Convey;
using Convey.Logging;
using Optimus.Services.Customers.Application;
using Optimus.Services.Customers.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConvey()
    .AddApplication()
    .AddInfrastructure();

builder.WebHost.UseLogging();

var app = builder.Build();

app.UseInfrastructure()
    .UseRouting()
    .UseEndpoints(e => e.MapControllers());

app.Run();