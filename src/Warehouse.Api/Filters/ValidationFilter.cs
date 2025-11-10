using FluentValidation;

namespace Warehouse.Api.Filters
{
    public sealed class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
        {
            var validators = ctx.HttpContext.RequestServices.GetService<IEnumerable<IValidator<T>>>();
            var arg = ctx.Arguments.OfType<T>().FirstOrDefault();
            if (validators is not null && arg is not null)
            {
                var failures = validators.Select(v => v.Validate(arg)).SelectMany(r => r.Errors).Where(f => f is not null).ToList();
                if (failures.Count > 0)
                {
                    var dict = failures.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
                    return Results.ValidationProblem(dict);
                }
            }
            return await next(ctx);
        }
    }
}
