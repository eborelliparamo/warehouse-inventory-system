using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Domain.Events;
using Warehouse.Domain.Inventory;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed class CreateItemCommandHandler(IInventoryWriteRepository repo, IDomainEventCollector events, WarehouseWriteDbContext db) : ICommandHandler<CreateItemCommand>
    {
        public async Task Handle(CreateItemCommand cmd, CancellationToken ct)
        {
            var existing = await repo.GetBySkuAsync(cmd.Sku, ct);
            if (existing is not null) return;

            var entity = InventoryItem.CreateOrThrow(cmd.Sku, cmd.Name);
            await repo.AddAsync(entity, ct);

            events.Add(new ItemCreated(entity.Sku, entity.Name, DateTime.UtcNow));
            await db.SaveChangesAsync(ct);
        }
    }
}
