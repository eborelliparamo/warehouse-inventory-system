using Microsoft.Extensions.DependencyInjection;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection s)
        {
            s.AddScoped<IInventoryWriteRepository, InventoryRepository>();
            s.AddScoped<IInventoryReadRepository, InventoryRepository>();

            return s;
        }
    }
}
