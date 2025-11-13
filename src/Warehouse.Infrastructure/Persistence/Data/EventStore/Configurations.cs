using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Warehouse.Infrastructure.Persistence.Data.EventStore
{

    public sealed class EventStreamCfg : IEntityTypeConfiguration<EventStream>
    {
        public void Configure(EntityTypeBuilder<EventStream> b)
        {
            b.ToTable("event_stream");
            b.HasKey(x => x.StreamId);
            b.Property(x => x.AggregateType).HasMaxLength(128).IsRequired();
            b.Property(x => x.Sku).IsRequired();
            b.HasIndex(x => x.Sku).IsUnique();
            b.Property(x => x.Version).IsConcurrencyToken();
            b.Property(x => x.CreatedAt).HasColumnName("created_at");
        }
    }
    public sealed class EventRecordCfg : IEntityTypeConfiguration<EventRecord>
    {
        public void Configure(EntityTypeBuilder<EventRecord> b)
        {
            b.ToTable("event");
            b.HasKey(x => x.EventId);
            b.HasOne(x => x.Stream).WithMany(s => s.Events).HasForeignKey(x => x.StreamId)
             .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.Type).HasMaxLength(128).IsRequired();
            b.Property(x => x.Data).HasColumnType("jsonb");
            b.Property(x => x.Metadata).HasColumnType("jsonb");
            b.Property(x => x.OccurredAt).HasColumnName("occurred_at");

            b.HasIndex(x => new { x.StreamId, x.Version }).IsUnique();
            b.HasIndex(x => x.StreamId);
        }
    }
}

