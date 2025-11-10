using Warehouse.Domain;
using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Infrastructure.ReadModel.Projectors
{
    public sealed class ItemCreatedProjector : EventProjector<ItemCreated>
    {
        protected override async Task ProjectAsync(WarehouseReadDbContext db, ItemCreated e, CancellationToken ct)
        {
            var sku = e.Sku.Value;
            if (await db.ItemSummary.FindAsync(sku, ct) is null)
                db.ItemSummary.Add(new ItemSummary { Sku = sku, Name = e.Name, Quantity = 0, LowStockThreshold = 10 });
        }
    }
}
