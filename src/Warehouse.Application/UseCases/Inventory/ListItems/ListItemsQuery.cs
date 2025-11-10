using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.Dtos;

namespace Warehouse.Application.UseCases.Inventory.ListItems
{
    public sealed record ListItemsQuery() : IQuery<IReadOnlyList<ItemListDto>>;
}
