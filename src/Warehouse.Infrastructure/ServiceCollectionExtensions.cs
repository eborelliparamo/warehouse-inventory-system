using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Persistence.Interceptors;
using Warehouse.Infrastructure.ReadModel;
using Warehouse.Infrastructure.ReadModel.Projectors;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IInventoryWriteRepository, InventoryRepository>();
            services.AddScoped<IInventoryReadRepository, InventoryRepository>();
            services.AddScoped<IEventProjector, ItemCreatedProjector>();
            services.AddScoped<IEventProjector, StockInProjector>();
            services.AddScoped<IEventProjector, StockOutProjector>();

            services.AddScoped<IEventProjectors, EventProjectors>();

            services.AddScoped<PostCommitReadModelInterceptor>();
            return services;
        }
    }
}
