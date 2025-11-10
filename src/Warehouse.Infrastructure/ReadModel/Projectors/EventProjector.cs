using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Infrastructure.ReadModel.Projectors
{
    public abstract class EventProjector<TEvent> : IEventProjector where TEvent : IDomainEvent
    {
        public Type EventType => typeof(TEvent);

        public Task ProjectAsync(WarehouseReadDbContext db, IDomainEvent evt, CancellationToken ct)
            => ProjectAsync(db, (TEvent)evt, ct);

        protected abstract Task ProjectAsync(WarehouseReadDbContext db, TEvent evt, CancellationToken ct);
    }
}
