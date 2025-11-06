using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Api.Read.Modules
{
    public static class InventoryModule
    {
        public static IEndpointRouteBuilder MapInventoryModule(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/inventory").WithTags("Inventory");

            group.MapGet("/low-stock", async (WarehouseReadDbContext db, int threshold = 10) =>
                await db.ItemSummary
                    .AsNoTracking()
                    .Where(x => x.Quantity < threshold)
                    .OrderBy(x => x.Quantity)
                    .ToListAsync());

            group.MapGet("/audit", async (WarehouseReadDbContext db, int take = 200) =>
                await db.AuditLog
                    .AsNoTracking()
                    .OrderByDescending(x => x.OccurredAt)
                    .Take(take)
                    .ToListAsync());

            return app;
        }

    }
}
