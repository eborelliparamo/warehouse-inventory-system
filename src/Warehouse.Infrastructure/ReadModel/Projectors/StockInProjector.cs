using Warehouse.Domain;
using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Infrastructure.ReadModel.Projectors
{
    public sealed class StockInProjector : EventProjector<StockInRegistered>
    {
        protected override async Task ProjectAsync(WarehouseReadDbContext db, StockInRegistered e, CancellationToken ct)
        {
            var sku = e.Sku.Value;
            var qty = e.Quantity.Value;

            var row = await db.ItemSummary.FindAsync(sku, ct)
                      ?? throw new InvalidOperationException($"Missing ItemSummary for SKU '{sku}'.");

            row.Quantity += qty;
            db.AuditLog.Add(new AuditLogRow { Id = Guid.NewGuid(), Sku = sku, Delta = qty, OccurredAt = e.OccurredAtUtc });
        }
    }
}
