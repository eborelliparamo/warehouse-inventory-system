using Warehouse.Application.Abstractions.Inventory;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.GetItemBySku
{
    public sealed class GetItemBySkuHandler(IInventoryReadRepository read) : IQueryHandler<GetItemBySkuQuery, ItemDetailsDto?>
    {
        public Task<ItemDetailsDto?> Handle(GetItemBySkuQuery query, CancellationToken ct) => read.GetBySkuAsync(
            query.Sku,
            static (InventoryItem x) => new ItemDetailsDto(x.Sku, x.Name, x.TotalQuantity),
            ct);
    }
}
