using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;
using Warehouse.Infrastructure.Persistence.Data.EventStore;

namespace Warehouse.Infrastructure.Persistence.Data
{
    public sealed class WarehouseWriteDbContext(DbContextOptions<WarehouseWriteDbContext> options) : DbContext(options)
    {
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<EventStream> EventStreams => Set<EventStream>();
        public DbSet<EventRecord> Events => Set<EventRecord>();
        public DbSet<Warehouse.Infrastructure.Persistence.Data.Outbox.OutboxRow> Outbox => Set<Warehouse.Infrastructure.Persistence.Data.Outbox.OutboxRow>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.ApplyConfiguration(new EventStreamCfg());
            b.ApplyConfiguration(new EventRecordCfg());

            var qtyConverter = new ValueConverter<InventoryQuantity, int>(
                v => v.Value, v => new InventoryQuantity(v));

            var skuConverter = new ValueConverter<Sku, string>(
                v => v.Value, v => new Sku(v));

            b.HasDefaultSchema("public");

            var e = b.Entity<InventoryItem>();
            e.ToTable("inventory_item");
            e.HasKey(x => x.Id);

            e.Property(x => x.Sku)
             .HasConversion(skuConverter)
             .IsRequired();

            e.HasIndex(x => x.Sku).IsUnique();

            e.Property(x => x.Name).IsRequired();

            e.Property(x => x.TotalQuantity)
             .HasConversion(qtyConverter);

            e.Property<DateTime>("CreatedAt")
             .HasDefaultValueSql("now()");

            b.Entity<Outbox.OutboxRow>(e =>
            {
                e.ToTable("outbox");
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.StreamId, x.Version }).IsUnique();
            });
        }
    }
}
