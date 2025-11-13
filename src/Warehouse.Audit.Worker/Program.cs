using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Warehouse.Audit.Infrastructure;
using Warehouse.Audit.Worker;


var builder = Host.CreateApplicationBuilder(args);

var cs = builder.Configuration.GetConnectionString("Audit")
         ?? builder.Configuration["ConnectionStrings:Audit"];

builder.Services.AddDbContext<AuditDbContext>(o =>
    o.UseNpgsql(cs).UseSnakeCaseNamingConvention());

builder.Services.AddSingleton<IConnection>(_ =>
{
    var host = builder.Configuration["Rabbit:Host"] ?? "localhost";
    return new ConnectionFactory { HostName = host, DispatchConsumersAsync = true }.CreateConnection();
});

builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();