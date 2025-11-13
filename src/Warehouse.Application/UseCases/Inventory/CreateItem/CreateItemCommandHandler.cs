using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Common;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed class CreateItemCommandHandler(IInventoryEventSourcedRepository repo, IUnitOfWork uow) : ICommandHandler<CreateItemCommand>
    {
        public async Task Handle(CreateItemCommand cmd, CancellationToken ct)
        {
            await uow.ExecuteAsync(async _ =>
            {
                var agg = await repo.GetAsync(cmd.Sku, ct);
                if (agg.Version > 0) return;
                agg = InventoryItemAggregate.CreateNew(cmd.Sku, cmd.Name, Guid.NewGuid());
                await repo.SaveAsync(agg, ct);
            }, ct);
        }
    }
}
