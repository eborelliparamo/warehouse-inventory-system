using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Warehouse.Infrastructure.Persistence.Data.EventStore
{
    public sealed class EventRecord
    {
        [Key] public Guid EventId { get; set; }
        public Guid StreamId { get; set; }
        public long Version { get; set; }
        public string Type { get; set; } = default!;
        public JsonDocument Data { get; set; } = default!;
        public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");
        public DateTime OccurredAt { get; set; }

        public EventStream Stream { get; set; } = default!;
    }
}
