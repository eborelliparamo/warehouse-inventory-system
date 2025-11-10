using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Infrastructure.ReadModel.Projectors
{
    public interface IEventProjector
    {
        Type EventType { get; }
        Task ProjectAsync(WarehouseReadDbContext db, IDomainEvent evt, CancellationToken ct);
    }
}
