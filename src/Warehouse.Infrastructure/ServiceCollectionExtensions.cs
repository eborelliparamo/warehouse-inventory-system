using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.Persistence.Interceptors;
using Warehouse.Infrastructure.ReadModel;
using Warehouse.Infrastructure.ReadModel.Projectors;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureWrite(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<PostCommitReadModelInterceptor>();

            services.AddDbContext<WarehouseWriteDbContext>((sp, o) =>
            o.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
             .AddInterceptors(sp.GetRequiredService<PostCommitReadModelInterceptor>()));

            services.AddDbContext<WarehouseReadDbContext>(o => o.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddScoped<IInventoryWriteRepository, InventoryRepository>();
            services.AddScoped<IInventoryReadRepository, InventoryRepository>();
            services.AddScoped<IEventProjector, ItemCreatedProjector>();
            services.AddScoped<IEventProjector, StockInProjector>();
            services.AddScoped<IEventProjector, StockOutProjector>();
            services.AddScoped<IEventProjectors, EventProjectors>();

            return services;
        }

        public static IServiceCollection AddInfrastructureWrite(this IServiceCollection services, IConfiguration cfg)
            => services.AddInfrastructureWrite(
                cfg.GetConnectionString("Warehouse")
                ?? cfg["ConnectionStrings:Warehouse"]
                ?? throw new InvalidOperationException("Missing connection string 'Warehouse'."));

        public static IServiceCollection AddInfrastructureRead(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<WarehouseReadDbContext>(o =>
                o.UseNpgsql(connectionString)
                 .UseSnakeCaseNamingConvention()
                 .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            return services;
        }

        public static IServiceCollection AddInfrastructureRead(this IServiceCollection services, IConfiguration cfg)
            => services.AddInfrastructureRead(
                cfg.GetConnectionString("Warehouse")
                ?? cfg["ConnectionStrings:Warehouse"]
                ?? throw new InvalidOperationException("Missing connection string 'Warehouse'."));
    }
}
