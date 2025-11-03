using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Infrastructure.Data
{
    public sealed class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
    {
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            var qtyConverter = new ValueConverter<InventoryQuantity, int>(
                v => v.Value, v => new InventoryQuantity(v));

            var skuConverter = new ValueConverter<Sku, string>(
                v => v.Value, v => new Sku(v));

            b.HasDefaultSchema("public");

            var e = b.Entity<InventoryItem>();
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
        }
    }
}
