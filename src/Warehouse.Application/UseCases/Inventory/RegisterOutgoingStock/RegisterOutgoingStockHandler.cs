using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock
{
    public sealed class RegisterOutgoingStockHandler(IInventoryWriteRepository repo, WarehouseDbContext db) : ICommandHandler<RegisterOutgoingStockCommand>
    {
        public async Task Handle(RegisterOutgoingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterOutgoing(command.Quantity);
            await repo.UpdateAsync(item, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
