using Warehouse.Application.Abstractions;

namespace Warehouse.Infrastructure.Data
{
    public sealed class EfUnitOfWork(WarehouseDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    }
}
