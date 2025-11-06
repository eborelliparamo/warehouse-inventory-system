using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed class RegisterIncomingStockHandler(IInventoryWriteRepository repo, IDomainEventCollector events, WarehouseWriteDbContext db) : ICommandHandler<RegisterIncomingStockCommand>
    {
        public async Task Handle(RegisterIncomingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterIncoming(command.Quantity);

            events.Add(new StockInRegistered(item.Sku, command.Quantity, DateTime.UtcNow));
            await db.SaveChangesAsync(ct);
        }
    }
}
