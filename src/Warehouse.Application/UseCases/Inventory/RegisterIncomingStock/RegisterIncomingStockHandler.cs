using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed class RegisterIncomingStockHandler(IInventoryWriteRepository repo, WarehouseWriteDbContext db) : ICommandHandler<RegisterIncomingStockCommand>
    {
        public async Task Handle(RegisterIncomingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterIncoming(command.Quantity);

            await db.SaveChangesAsync(ct);
        }
    }
}
