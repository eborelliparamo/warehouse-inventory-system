using Warehouse.Api.DependencyInjection;
using Warehouse.Api.Modules;
using Warehouse.Api.Pipelines;
using System.Text.Json.Serialization.Metadata;
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

var app = builder.Build();
app.UseApiPipeline();
app.MapInventoryModule();

app.Run();