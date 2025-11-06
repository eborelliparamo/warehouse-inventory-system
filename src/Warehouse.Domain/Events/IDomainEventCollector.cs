namespace Warehouse.Domain.Events
{
    public interface IDomainEventCollector
    {
        void Add(IDomainEvent evt);
        IReadOnlyList<IDomainEvent> Drain();
    }
}
