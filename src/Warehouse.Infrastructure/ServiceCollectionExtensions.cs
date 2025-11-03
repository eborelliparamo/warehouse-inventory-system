using Microsoft.Extensions.DependencyInjection;
using Warehouse.Application.Abstractions;
using Warehouse.Application.Abstractions.Inventory;
using Warehouse.Infrastructure.Data;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection s)
        {
            s.AddScoped<IInventoryWriteRepository, InventoryRepository>();
            s.AddScoped<IInventoryReadRepository, InventoryRepository>();

            s.AddScoped<IUnitOfWork, EfUnitOfWork>();
            return s;
        }
    }
}
