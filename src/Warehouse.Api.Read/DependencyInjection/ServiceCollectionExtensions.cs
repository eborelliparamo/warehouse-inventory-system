namespace Warehouse.Api.Read.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApi(this IServiceCollection s)
        {
            s.AddEndpointsApiExplorer();
            s.AddHealthChecks();
            return s;
        }
    }
}
