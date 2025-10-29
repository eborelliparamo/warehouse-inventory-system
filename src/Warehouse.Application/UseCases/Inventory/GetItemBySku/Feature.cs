using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.Ports.Inventory;
using Warehouse.Application.UseCases.Inventory.Dtos;

namespace Warehouse.Application.UseCases.Inventory.GetItemBySku
{
    public sealed record GetItemBySkuQuery(string Sku) : IQuery<ItemDetailsDto?>;

    public sealed class Handler(IInventoryReadRepository read) : IQueryHandler<GetItemBySkuQuery, ItemDetailsDto?>
    {
        public Task<ItemDetailsDto?> Handle(GetItemBySkuQuery query, CancellationToken ct) => read.GetDetailsAsync(query.Sku, ct);
    }
}
