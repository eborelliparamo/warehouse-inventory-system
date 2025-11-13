using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Warehouse.Audit.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Audit")
         ?? builder.Configuration["ConnectionStrings:Audit"];

builder.Services.AddDbContext<AuditDbContext>(o =>
    o.UseNpgsql(cs).UseSnakeCaseNamingConvention()
     .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Audit API", Version = "v1" }));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Audit API v1"));

app.MapGet("/time-machine", async (AuditDbContext db, string sku, DateTime at, HttpContext http) =>
{
    var ct = http.RequestAborted;
    var balance = await db.AuditEvents
        .Where(x => x.Sku == sku && x.OccurredAt <= at)
        .SumAsync(x => (int?)x.Delta, ct) ?? 0;
    return Results.Ok(new { sku, at, balance });
})
.WithTags("Audit");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();