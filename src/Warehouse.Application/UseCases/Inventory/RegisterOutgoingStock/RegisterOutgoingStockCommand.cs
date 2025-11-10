using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock
{
    public sealed record RegisterOutgoingStockCommand(Sku Sku, MovementQuantity Quantity) : ICommand;
}
