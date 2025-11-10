using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.ReadModel.Projectors;

namespace Warehouse.Infrastructure.ReadModel
{
    public sealed class EventProjectors : IEventProjectors
    {
        private readonly Dictionary<Type, Func<WarehouseReadDbContext, IDomainEvent, CancellationToken, Task>> _map = new();

        public EventProjectors(IEnumerable<IEventProjector> projectors)
        {
            foreach (var p in projectors)
                _map[p.EventType] = p.ProjectAsync;
        }

        public bool TryGet(Type t,
            out Func<WarehouseReadDbContext, IDomainEvent, CancellationToken, Task> h)
            => _map.TryGetValue(t, out h!);
    }
}
