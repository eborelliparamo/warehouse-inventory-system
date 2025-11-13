using System.ComponentModel.DataAnnotations;

namespace Warehouse.Infrastructure.Persistence.Data.Outbox
{
    public sealed class OutboxRow
    {
        [Key] public Guid Id { get; set; }
        public string Kind { get; set; } = default!;
        public DateTime OccurredAt { get; set; }
        public Guid StreamId { get; set; }
        public long Version { get; set; }
        public byte[] Payload { get; set; } = default!;
        public string ContentType { get; set; } = "application/x-protobuf";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }
        public int Attempts { get; set; }
    }
}
