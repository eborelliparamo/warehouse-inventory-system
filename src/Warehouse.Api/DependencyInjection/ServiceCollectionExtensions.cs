using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Warehouse.Api.Middleware;
using Warehouse.Application.Cqrs;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.Ports.Inventory;
using Warehouse.Application.UseCases.Inventory.CreateItem;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Application.UseCases.Inventory.GetItemBySku;
using Warehouse.Application.UseCases.Inventory.ListItems;
using Warehouse.Application.UseCases.Inventory.RegisterIncomingStock;
using Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock;
using Warehouse.Infrastructure.Data;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProcessor, Processor>();

            services.AddScoped<ICommandHandler<CreateItemCommand>, Application.UseCases.Inventory.CreateItem.Handler>();
            services.AddScoped<ICommandHandler<RegisterIncomingStockCommand>, Application.UseCases.Inventory.RegisterIncomingStock.Handler>();
            services.AddScoped<ICommandHandler<RegisterOutgoingStockCommand>, Application.UseCases.Inventory.RegisterOutgoingStock.Handler>();
            services.AddScoped<IQueryHandler<GetItemBySkuQuery, ItemDetailsDto?>, Application.UseCases.Inventory.GetItemBySku.Handler>();
            services.AddScoped<IQueryHandler<ListItemsQuery, IReadOnlyList<ItemListDto>>, Application.UseCases.Inventory.ListItems.Handler>();

            // Validators
            services.AddValidatorsFromAssemblyContaining<CreateItemCommand>();
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("Warehouse")!;
            s.AddDbContext<WarehouseDbContext>(o => o.UseNpgsql(cs));
            s.AddScoped<IInventoryWriteRepository, InventoryWriteRepository>();
            s.AddScoped<IInventoryReadRepository, InventoryReadRepository>();
            return s;
        }

        public static IServiceCollection AddApi(this IServiceCollection s)
        {
            s.AddEndpointsApiExplorer();
            s.AddSwaggerGen();
            s.AddHealthChecks();
            return s;
        }
    }
}
