using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Warehouse.Domain.Events;
using Warehouse.Infrastructure.EventSourcing;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.Persistence.Data.EventStore;
using Warehouse.Infrastructure.Persistence.Data.Outbox;

namespace Warehouse.Infrastructure.Persistence.Interceptors
{
    public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IEventSerializer _ser;

        public OutboxSaveChangesInterceptor(IEventSerializer ser) => _ser = ser;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken ct = default)
        {
            if (eventData.Context is not WarehouseWriteDbContext db)
                return ValueTask.FromResult(result);

            var pending = db.ChangeTracker
                .Entries<EventRecord>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            if (pending.Count == 0)
                return ValueTask.FromResult(result);

            foreach (var ev in pending)
            {
                var envelope = new Contracts.Inventory.InventoryEventEnvelope
                {
                    EventType = ev.Type,
                    StreamId = ev.StreamId.ToString(),
                    Version = ev.Version,
                    OccurredAt = Timestamp.FromDateTime(ev.OccurredAt.ToUniversalTime())
                };

                switch (ev.Type)
                {
                    case nameof(ItemCreated):
                        {
                            var dto = _ser.Deserialize<ItemCreatedDto>(ev.Data);
                            envelope.ItemCreated = new Contracts.Inventory.ItemCreated
                            {
                                Sku = dto.sku,
                                Name = dto.name
                            };
                            break;
                        }

                    case nameof(StockInRegistered):
                        {
                            var dto = _ser.Deserialize<StockInRegisteredDto>(ev.Data);
                            envelope.StockInRegistered = new Contracts.Inventory.StockInRegistered
                            {
                                Sku = dto.sku,
                                Quantity = dto.quantity
                            };
                            break;
                        }

                    case nameof(StockOutRegistered):
                        {
                            var dto = _ser.Deserialize<StockOutRegisteredDto>(ev.Data);
                            envelope.StockOutRegistered = new Contracts.Inventory.StockOutRegistered
                            {
                                Sku = dto.sku,
                                Quantity = dto.quantity
                            };
                            break;
                        }

                    default:
                        continue;
                }

                var bytes = envelope.ToByteArray();
                db.Outbox.Add(new OutboxRow
                {
                    Id = Guid.NewGuid(),
                    Kind = ev.Type,
                    OccurredAt = ev.OccurredAt,
                    StreamId = ev.StreamId,
                    Version = ev.Version,
                    Payload = bytes,
                    ContentType = "application/x-protobuf"
                });
            }

            return ValueTask.FromResult(result);
        }
    }
}
