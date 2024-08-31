using Microsoft.EntityFrameworkCore;
using Optimus.Services.Worker;
using Optimus.Services.Worker.Infrastructure;
using Optimus.Services.Worker.Infrastructure.Postgres;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICrmService, CrmService>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<ChatsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();