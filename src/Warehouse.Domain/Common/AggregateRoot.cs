using Warehouse.Domain.Events;

namespace Warehouse.Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<IDomainEvent> _uncommitted = new();
        public IReadOnlyList<IDomainEvent> Uncommitted => _uncommitted;
        public long Version { get; protected set; } = 0;
        public Guid StreamId { get; protected set; }

        protected void Raise(IDomainEvent e) { When(e); _uncommitted.Add(e); }
        public void ClearUncommitted() => _uncommitted.Clear();

        public void LoadFromHistory(IEnumerable<IDomainEvent> history, long startVersion = 0, Guid? streamId = null)
        {
            StreamId = streamId ?? StreamId;
            foreach (var e in history) When(e);
            Version = startVersion + history.LongCount();
            _uncommitted.Clear();
        }

        protected abstract void When(IDomainEvent e);
    }
}
