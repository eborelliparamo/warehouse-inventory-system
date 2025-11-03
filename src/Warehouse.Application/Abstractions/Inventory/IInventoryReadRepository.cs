using System.Linq.Expressions;
using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Application.Abstractions.Inventory
{
    public interface IInventoryReadRepository
    {
        Task<T?> GetBySkuAsync<T>(Sku sku, Expression<Func<InventoryItem, T>> projector, CancellationToken ct);
        Task<IReadOnlyList<T>> ListAsync<T>(
            Expression<Func<InventoryItem, bool>>? filter,
            Expression<Func<InventoryItem, T>> projector,
            (Expression<Func<InventoryItem, object>> key, bool desc)? orderBy,
            CancellationToken ct);
    }
}
