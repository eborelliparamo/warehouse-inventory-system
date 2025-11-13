using System.ComponentModel.DataAnnotations;

namespace Warehouse.Infrastructure.Persistence.Data.EventStore
{
    public sealed class EventStream
    {
        [Key] public Guid StreamId { get; set; }
        public string AggregateType { get; set; } = default!;
        public string Sku { get; set; } = default!;
        [ConcurrencyCheck] public long Version { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EventRecord> Events { get; set; } = [];
    }
}
