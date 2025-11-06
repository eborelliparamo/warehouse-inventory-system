using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization.Metadata;
using Warehouse.Api.Read.DependencyInjection;
using Warehouse.Api.Read.Modules;
using Warehouse.Api.Read.Pipelines;
using Warehouse.Application;
using Warehouse.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
});

builder.Services
    .AddApi(builder.Configuration)
    .AddApplication()
    .AddInfrastructure();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Warehouse Read API",
        Version = "v1",
        Description = "Read-side (proyecciones) – low stock & audit"
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse Read API v1");
    c.DocumentTitle = "Warehouse Read Swagger";
});
app.UseApiPipeline();
app.MapInventoryModule();

app.Run();