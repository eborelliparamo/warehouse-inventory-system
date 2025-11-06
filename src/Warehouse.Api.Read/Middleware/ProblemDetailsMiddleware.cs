using System.Net.Mime;

namespace Warehouse.Api.Read.Middleware
{
    public sealed class ProblemDetailsMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try { await next(context); }
            catch (Exception ex)
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;
                var (status, title) = ex switch
                {
                    InvalidOperationException => (StatusCodes.Status409Conflict, ex.Message),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                    ArgumentException or ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, ex.Message),
                    _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
                };
                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(new { title, status });
            }
        }
    }
}
