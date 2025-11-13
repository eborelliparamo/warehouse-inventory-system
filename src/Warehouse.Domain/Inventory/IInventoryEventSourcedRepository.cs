using Warehouse.Domain.ValueObjects;

namespace Warehouse.Domain.Inventory
{
    public interface IInventoryEventSourcedRepository
    {
        Task<InventoryItemAggregate> GetAsync(Sku sku, CancellationToken ct);
        Task SaveAsync(InventoryItemAggregate agg, CancellationToken ct);
    }
}
