using Warehouse.Domain.Events;
using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;
using Warehouse.Infrastructure.EventSourcing;

namespace Warehouse.Infrastructure.Repositories
{
    public sealed class InventoryEventSourcedRepository : IInventoryEventSourcedRepository
    {
        private readonly IEventStore _store;

        public InventoryEventSourcedRepository(IEventStore store) => _store = store;

        public async Task<InventoryItemAggregate> GetAsync(Sku sku, CancellationToken ct)
        {
            var (stream, _) = await _store.GetOrCreateStreamAsync(sku, ct);
            var history = await _store.LoadAsync(stream.StreamId, 0, ct);

            var agg = new InventoryItemAggregate();
            agg.LoadFromHistory(history, 0, stream.StreamId);
            return agg;
        }

        public async Task SaveAsync(InventoryItemAggregate agg, CancellationToken ct)
        {
            var uncommitted = agg.Uncommitted.ToArray();
            if (uncommitted.Length == 0) return;

            var (stream, _) = await _store.GetOrCreateStreamAsync(agg.Sku, ct);

            await _store.AppendAsync(stream, expectedVersion: agg.Version, events: uncommitted, ct);

            agg.LoadFromHistory(Array.Empty<IDomainEvent>(), agg.Version + uncommitted.Length, stream.StreamId);
            agg.ClearUncommitted();
        }
    }
}
