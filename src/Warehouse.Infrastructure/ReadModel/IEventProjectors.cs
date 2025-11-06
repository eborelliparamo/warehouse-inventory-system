using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Infrastructure.ReadModel
{
    public interface IEventProjectors
    {
        bool TryGet(Type eventType, out Func<WarehouseReadDbContext, IDomainEvent, CancellationToken, Task> handler);
    }
}
