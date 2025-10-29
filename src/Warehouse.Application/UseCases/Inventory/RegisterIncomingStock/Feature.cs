using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.Ports.Inventory;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed record RegisterIncomingStockCommand(string Sku, int Quantity) : ICommand;

    public sealed class Handler(IInventoryWriteRepository repo) : ICommandHandler<RegisterIncomingStockCommand>
    {
        public async Task Handle(RegisterIncomingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterIncoming(command.Quantity);
            await repo.UpdateAsync(item, ct);
        }
    }
}
