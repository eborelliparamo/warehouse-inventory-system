using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApi(this IServiceCollection s, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("Warehouse")!;
            s.AddDbContext<WarehouseDbContext>(o => o.UseNpgsql(cs).UseSnakeCaseNamingConvention());
            s.AddEndpointsApiExplorer();
            s.AddSwaggerGen();
            s.AddHealthChecks();
            return s;
        }
    }
}
