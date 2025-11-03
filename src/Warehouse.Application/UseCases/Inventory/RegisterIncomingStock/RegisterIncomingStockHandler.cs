using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed class RegisterIncomingStockHandler(IInventoryWriteRepository repo, WarehouseDbContext db) : ICommandHandler<RegisterIncomingStockCommand>
    {
        public async Task Handle(RegisterIncomingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterIncoming(command.Quantity);
            await repo.UpdateAsync(item, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
