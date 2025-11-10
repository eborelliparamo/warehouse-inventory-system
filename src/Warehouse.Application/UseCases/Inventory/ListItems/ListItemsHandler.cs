using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.ListItems
{
    public sealed class ListItemsHandler(IInventoryReadRepository read) : IQueryHandler<ListItemsQuery, IReadOnlyList<ItemListDto>>
    {
        public Task<IReadOnlyList<ItemListDto>> Handle(ListItemsQuery query, CancellationToken ct)
                    => read.ListAsync(
            filter: null,
            projector: static (InventoryItem x) => new ItemListDto(x.Sku, x.Name, x.TotalQuantity),
            orderBy: (x => x.Sku, desc: false),
            ct);
    }
}
