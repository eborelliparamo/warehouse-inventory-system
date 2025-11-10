using Warehouse.Api.DependencyInjection;
using Warehouse.Api.Modules;
using Warehouse.Api.Pipelines;
using System.Text.Json.Serialization.Metadata;
using Warehouse.Application;
using Warehouse.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
});

builder.Services
    .AddApi()
    .AddApplication()
    .AddInfrastructureWrite(builder.Configuration);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Warehouse Write API",
        Version = "v1",
        Description = "Write-side (commands) – create item, stock in/out"
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse Write API v1");
    c.DocumentTitle = "Warehouse Write Swagger";
});
app.UseApiPipeline();
app.MapInventoryModule();

app.Run();