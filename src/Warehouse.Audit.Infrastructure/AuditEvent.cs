namespace Warehouse.Audit.Infrastructure
{
    public sealed class AuditEvent
    {
        public Guid Id { get; set; }
        public Guid StreamId { get; set; }
        public long Version { get; set; }
        public string Sku { get; set; } = default!;
        public string Type { get; set; } = default!;
        public int Delta { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
