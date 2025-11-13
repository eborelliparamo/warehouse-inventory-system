using Warehouse.Domain.Events;
using Warehouse.Domain.ValueObjects;
using Warehouse.Infrastructure.Persistence.Data.EventStore;

namespace Warehouse.Infrastructure.EventSourcing
{
    public interface IEventStore
    {
        Task<(EventStream Stream, long Version)> GetOrCreateStreamAsync(Sku sku, CancellationToken ct);
        Task<IReadOnlyList<IDomainEvent>> LoadAsync(Guid streamId, long fromVersionExclusive, CancellationToken ct);
        Task AppendAsync(EventStream stream, long expectedVersion, IReadOnlyList<IDomainEvent> events, CancellationToken ct);
    }
}
