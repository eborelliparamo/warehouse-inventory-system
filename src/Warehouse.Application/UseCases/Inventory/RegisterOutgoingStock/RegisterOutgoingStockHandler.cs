using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock
{
    public sealed class RegisterOutgoingStockHandler(IInventoryWriteRepository repo, WarehouseWriteDbContext db) : ICommandHandler<RegisterOutgoingStockCommand>
    {
        public async Task Handle(RegisterOutgoingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterOutgoing(command.Quantity);
            await db.SaveChangesAsync(ct);
        }
    }
}
