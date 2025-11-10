using Microsoft.EntityFrameworkCore;
using Warehouse.Domain;

namespace Warehouse.Infrastructure.Persistence.Data
{
    public sealed class WarehouseReadDbContext(DbContextOptions<WarehouseReadDbContext> options) : DbContext(options)
    {
        public DbSet<ItemSummary> ItemSummary => Set<ItemSummary>();
        public DbSet<AuditLogRow> AuditLog => Set<AuditLogRow>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<ItemSummary>(e =>
            {
                e.ToTable("item_summary");
                e.HasKey(x => x.Sku);
            });

            b.Entity<AuditLogRow>(e =>
            {
                e.ToTable("audit_log");
                e.HasKey(x => x.Id);
            });
        }
    }
}
