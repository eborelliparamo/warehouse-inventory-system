using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Warehouse.Api.Read.Middleware;

namespace Warehouse.Api.Read.Pipelines
{
    public static class ApiPipelineExtensions
    {
        public static IApplicationBuilder UseApiPipeline(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
            app.UseMiddleware<ProblemDetailsMiddleware>();
            if (env.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
            app.UseHealthChecks("/health", new HealthCheckOptions());
            return app;
        }
    }
}
