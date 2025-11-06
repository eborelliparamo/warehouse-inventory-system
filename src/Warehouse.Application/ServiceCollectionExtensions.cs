using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Application.Common;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.CreateItem;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Application.UseCases.Inventory.GetItemBySku;
using Warehouse.Application.UseCases.Inventory.ListItems;
using Warehouse.Application.UseCases.Inventory.RegisterIncomingStock;
using Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock;
using Warehouse.Domain.Events;

namespace Warehouse.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<CreateItemCommand>, CreateItemCommandHandler>();
            services.AddScoped<ICommandHandler<RegisterIncomingStockCommand>, RegisterIncomingStockHandler>();
            services.AddScoped<ICommandHandler<RegisterOutgoingStockCommand>, RegisterOutgoingStockHandler>();
            services.AddScoped<IQueryHandler<GetItemBySkuQuery, ItemDetailsDto?>, GetItemBySkuHandler>();
            services.AddScoped<IQueryHandler<ListItemsQuery, IReadOnlyList<ItemListDto>>, ListItemsHandler>();

            services.AddScoped<IDomainEventCollector, EventCollector>();
            // Validators
            services.AddValidatorsFromAssemblyContaining<CreateItemCommand>();
            return services;
        }

    }
}
