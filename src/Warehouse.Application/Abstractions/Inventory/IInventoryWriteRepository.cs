using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Application.Abstractions.Inventory
{
    public interface IInventoryWriteRepository
    {
        Task<InventoryItem?> GetBySkuAsync(Sku sku, CancellationToken ct);
        Task AddAsync(InventoryItem entity, CancellationToken ct);
        Task UpdateAsync(InventoryItem entity, CancellationToken ct);
    }
}
