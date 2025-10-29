using Warehouse.Domain.Inventory;

namespace Warehouse.Application.Ports.Inventory
{
    public interface IInventoryWriteRepository
    {
        Task AddAsync(InventoryItem item, CancellationToken ct = default);
        Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct = default);
        Task UpdateAsync(InventoryItem item, CancellationToken ct = default);
    }
}
