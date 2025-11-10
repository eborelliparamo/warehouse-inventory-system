using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed record CreateItemCommand(Sku Sku, string Name) : ICommand;
}
