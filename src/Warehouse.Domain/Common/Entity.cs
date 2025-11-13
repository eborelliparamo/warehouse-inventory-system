using System.ComponentModel.DataAnnotations.Schema;
using Warehouse.Domain.Events;

namespace Warehouse.Domain.Common
{
    public abstract class Entity : IHaveDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
