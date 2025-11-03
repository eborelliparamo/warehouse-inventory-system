using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Repositories
{
    public sealed class InventoryRepository(WarehouseDbContext db) : IInventoryWriteRepository, IInventoryReadRepository
    {
        public Task<InventoryItem?> GetBySkuAsync(Sku sku, CancellationToken ct)
            => db.Set<InventoryItem>().SingleOrDefaultAsync(x => x.Sku == sku, ct);

        public Task AddAsync(InventoryItem entity, CancellationToken ct)
        { db.Set<InventoryItem>().Add(entity); return Task.CompletedTask; }

        public Task UpdateAsync(InventoryItem entity, CancellationToken ct)
        { db.Set<InventoryItem>().Update(entity); return Task.CompletedTask; }

        public Task<T?> GetBySkuAsync<T>(
            Sku sku,
            Expression<Func<InventoryItem, T>> projector,
            CancellationToken ct)
            => db.Set<InventoryItem>()
                 .AsNoTracking()
                 .Where(x => x.Sku == sku)
                 .Select(projector)
                 .SingleOrDefaultAsync(ct);

        public async Task<IReadOnlyList<T>> ListAsync<T>(
            Expression<Func<InventoryItem, bool>>? filter,
            Expression<Func<InventoryItem, T>> projector,
            (Expression<Func<InventoryItem, object>> key, bool desc)? orderBy,
            CancellationToken ct)
        {
            IQueryable<InventoryItem> q = db.Set<InventoryItem>().AsNoTracking();

            if (filter is not null)
                q = q.Where(filter);

            if (orderBy is not null)
                q = orderBy.Value.desc
                    ? q.OrderByDescending(orderBy.Value.key)
                    : q.OrderBy(orderBy.Value.key);

            return await q.Select(projector).ToListAsync(ct);
        }
    }

}
