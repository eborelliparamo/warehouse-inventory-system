using Warehouse.Application.Abstractions;
using Warehouse.Application.Abstractions.Inventory;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed class CreateItemCommandHandler(IInventoryWriteRepository repo, IUnitOfWork uow) : ICommandHandler<CreateItemCommand>
    {
        public async Task Handle(CreateItemCommand cmd, CancellationToken ct)
        {
            var existing = await repo.GetBySkuAsync(cmd.Sku, ct);
            if (existing is not null) return;

            var item = InventoryItem.Create(cmd.Sku, cmd.Name);

            await repo.AddAsync(item, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
}
