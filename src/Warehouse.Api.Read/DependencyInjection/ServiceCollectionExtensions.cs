using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.Persistence.Interceptors;

namespace Warehouse.Api.Read.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApi(this IServiceCollection s, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("Warehouse")!;

            s.AddDbContext<WarehouseWriteDbContext>((sp, o) =>
                o.UseNpgsql(cs).UseSnakeCaseNamingConvention()
                 .AddInterceptors(sp.GetRequiredService<PostCommitReadModelInterceptor>()));

            s.AddDbContext<WarehouseReadDbContext>(o => o.UseNpgsql(cs).UseSnakeCaseNamingConvention()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            s.AddEndpointsApiExplorer();
            s.AddHealthChecks();
            return s;
        }
    }
}
