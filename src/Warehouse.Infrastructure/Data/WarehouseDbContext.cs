using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Warehouse.Domain.Inventory;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Infrastructure.Data
{
    public sealed class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
    {
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var qtyConverter = new ValueConverter<InventoryQuantity, int>(
                                toProvider => toProvider.Value,
                                fromProvider => new InventoryQuantity(fromProvider)
                            );

            modelBuilder.HasDefaultSchema("public");
            var e = modelBuilder.Entity<InventoryItem>();
            e.ToTable("inventory_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Sku).HasColumnName("sku").IsRequired();
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.Name).HasColumnName("name").IsRequired();
            e.Property(x => x.TotalQuantity).HasColumnName("total_quantity").HasConversion(qtyConverter);
            e.Property<DateTime>("created_at");
        }
    }
}
