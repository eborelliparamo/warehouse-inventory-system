using Warehouse.Domain.Events;

namespace Warehouse.Application.Common
{
    public sealed class EventCollector : IDomainEventCollector
    {
        private readonly List<IDomainEvent> _events = new();
        public void Add(IDomainEvent evt) => _events.Add(evt);
        public IReadOnlyList<IDomainEvent> Drain()
        {
            var copy = _events.ToArray();
            _events.Clear();
            return copy;
        }
    }
}
