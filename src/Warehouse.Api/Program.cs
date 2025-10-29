using Warehouse.Api.DependencyInjection;
using Warehouse.Api.Modules;
using Warehouse.Api.Pipelines;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
});

builder.Services
.AddApplication()
.AddInfrastructure(builder.Configuration)
.AddApi(); 

var app = builder.Build();
app.UseApiPipeline();
app.MapInventoryModule();

app.Run();