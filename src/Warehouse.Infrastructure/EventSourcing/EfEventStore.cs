using Microsoft.EntityFrameworkCore;
using System.Data;
using Warehouse.Domain.Events;
using Warehouse.Domain.ValueObjects;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.Persistence.Data.EventStore;

namespace Warehouse.Infrastructure.EventSourcing
{
    public sealed class EfEventStore : IEventStore
    {
        private readonly WarehouseWriteDbContext _db;
        private readonly IEventSerializer _ser;

        public EfEventStore(WarehouseWriteDbContext db, IEventSerializer ser) => (_db, _ser) = (db, ser);

        public async Task<(EventStream, long)> GetOrCreateStreamAsync(Sku sku, CancellationToken ct)
        {
            var stream = await _db.EventStreams.SingleOrDefaultAsync(x => x.Sku == sku.Value, ct);
            if (stream is null)
            {
                stream = new EventStream
                {
                    StreamId = Guid.NewGuid(),
                    AggregateType = "InventoryItem",
                    Sku = sku.Value,
                    Version = 0
                };
                _db.EventStreams.Add(stream);
                await _db.SaveChangesAsync(ct);
            }
            return (stream, stream.Version);
        }

        public async Task<IReadOnlyList<IDomainEvent>> LoadAsync(Guid streamId, long fromVersionExclusive, CancellationToken ct)
        {
            var rows = await _db.Events
                .AsNoTracking()
                .Where(e => e.StreamId == streamId && e.Version > fromVersionExclusive)
                .OrderBy(e => e.Version)
                .ToListAsync(ct);

            return rows
                .Select(r => _ser.DeserializeByName(r.Type, r.Data))
                .ToList();
        }

        public async Task AppendAsync(EventStream stream, long expectedVersion, IReadOnlyList<IDomainEvent> events, CancellationToken ct)
        {
            if (events.Count == 0) return;

            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            if (stream.Version != expectedVersion)
                throw new DBConcurrencyException($"Expected v{expectedVersion} got v{stream.Version}");

            var next = expectedVersion;

            foreach (var e in events)
            {
                next++;
                var (type, data, meta, when) = _ser.Serialize(e);
                _db.Events.Add(new EventRecord
                {
                    EventId = Guid.NewGuid(),
                    StreamId = stream.StreamId,
                    Version = next,
                    Type = type,
                    Data = data,
                    Metadata = meta,
                    OccurredAt = when
                });
            }

            stream.Version = next;
            _db.EventStreams.Update(stream);

            try
            {
                await _db.SaveChangesAsync(ct); 
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex)
            {
                throw new DBConcurrencyException("Concurrency conflict on event append.", ex);
            }
        }
    }
}

