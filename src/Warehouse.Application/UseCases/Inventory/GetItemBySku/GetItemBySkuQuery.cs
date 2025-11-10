using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Application.UseCases.Inventory.GetItemBySku
{
    public sealed record GetItemBySkuQuery(Sku Sku) : IQuery<ItemDetailsDto?>;
}
