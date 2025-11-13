using Microsoft.EntityFrameworkCore;

namespace Warehouse.Audit.Infrastructure;

public sealed class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AuditEvent>(e =>
        {
            e.ToTable("audit_event");
            e.HasKey(x => x.Id);

            e.Property(x => x.StreamId).HasColumnName("stream_id");
            e.Property(x => x.Version).HasColumnName("version");
            e.Property(x => x.Sku).HasColumnName("sku");
            e.Property(x => x.Type).HasColumnName("type");
            e.Property(x => x.Delta).HasColumnName("delta");
            e.Property(x => x.OccurredAt).HasColumnName("occurred_at");

            e.HasIndex(x => new { x.Sku, x.OccurredAt });
            e.HasIndex(x => new { x.StreamId, x.Version }).IsUnique();
        });
    }
}

