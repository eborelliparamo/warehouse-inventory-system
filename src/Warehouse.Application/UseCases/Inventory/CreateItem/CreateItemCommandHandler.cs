using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Domain.Inventory;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed class CreateItemCommandHandler(IInventoryWriteRepository repo, WarehouseWriteDbContext db) : ICommandHandler<CreateItemCommand>
    {
        public async Task Handle(CreateItemCommand cmd, CancellationToken ct)
        {
            var existing = await repo.GetBySkuAsync(cmd.Sku, ct);
            if (existing is not null) return;

            var entity = InventoryItem.CreateOrThrow(cmd.Sku, cmd.Name);
            await repo.AddAsync(entity, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
