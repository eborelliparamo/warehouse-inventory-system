using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.Ports.Inventory;
using Warehouse.Application.UseCases.Inventory.Dtos;

namespace Warehouse.Application.UseCases.Inventory.ListItems
{
    public sealed record ListItemsQuery() : IQuery<IReadOnlyList<ItemListDto>>;

    public sealed class Handler(IInventoryReadRepository read) : IQueryHandler<ListItemsQuery, IReadOnlyList<ItemListDto>>
    {
        public Task<IReadOnlyList<ItemListDto>> Handle(ListItemsQuery query, CancellationToken ct) => read.ListAsync(ct);
    }
}
