using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Inventory;

namespace Warehouse.Infrastructure.Data
{
    public sealed class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
    {
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            var e = modelBuilder.Entity<InventoryItem>();
            e.ToTable("inventory_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Sku).HasColumnName("sku").IsRequired();
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.Name).HasColumnName("name").IsRequired();
            e.Property(x => x.TotalQuantity).HasColumnName("total_quantity");
            e.Property<DateTime>("created_at");
        }
    }
}
