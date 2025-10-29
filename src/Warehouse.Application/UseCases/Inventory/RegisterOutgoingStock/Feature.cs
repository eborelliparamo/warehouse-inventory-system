using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.Ports.Inventory;

namespace Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock
{
    public sealed record RegisterOutgoingStockCommand(string Sku, int Quantity) : ICommand;

    public sealed class Handler(IInventoryWriteRepository repo) : ICommandHandler<RegisterOutgoingStockCommand>
    {
        public async Task Handle(RegisterOutgoingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterOutgoing(command.Quantity);
            await repo.UpdateAsync(item, ct);
        }
    }
}
