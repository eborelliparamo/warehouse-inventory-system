using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed record RegisterIncomingStockCommand(Sku Sku, MovementQuantity Quantity) : ICommand;
}
