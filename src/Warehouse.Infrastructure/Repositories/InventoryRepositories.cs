using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Ports.Inventory;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Domain.Inventory;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Repositories
{
    public sealed class InventoryWriteRepository(WarehouseDbContext db) : IInventoryWriteRepository
    {
        public Task AddAsync(InventoryItem item, CancellationToken ct = default)
        => db.InventoryItems.AddAsync(item, ct).AsTask();


        public Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct = default)
        => db.InventoryItems.FirstOrDefaultAsync(x => x.Sku == sku, ct)!;


        public Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
        { db.InventoryItems.Update(item); return Task.CompletedTask; }
    }

    public sealed class InventoryReadRepository(WarehouseDbContext db) : IInventoryReadRepository
    {
        public Task<ItemDetailsDto?> GetDetailsAsync(string sku, CancellationToken ct = default)
        => db.InventoryItems.AsNoTracking()
        .Where(x => x.Sku == sku)
        .Select(x => new ItemDetailsDto(x.Sku, x.Name, x.TotalQuantity))
        .FirstOrDefaultAsync(ct)!;


        public async Task<IReadOnlyList<ItemListDto>> ListAsync(CancellationToken ct = default)
        => await db.InventoryItems.AsNoTracking()
        .OrderBy(x => x.Sku)
        .Select(x => new ItemListDto(x.Sku, x.Name, x.TotalQuantity))
        .ToListAsync(ct);
    }
}
