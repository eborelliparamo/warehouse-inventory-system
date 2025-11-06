namespace Warehouse.Domain
{
    public sealed class AuditLogRow
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = default!;
        public int Delta { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
