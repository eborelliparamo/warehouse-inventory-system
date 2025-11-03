using Warehouse.Application.Abstractions;
using Warehouse.Application.Abstractions.Inventory;
using Warehouse.Application.Cqrs.Abstractions;

namespace Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock
{
    public sealed class RegisterOutgoingStockHandler(IInventoryWriteRepository repo, IUnitOfWork uow) : ICommandHandler<RegisterOutgoingStockCommand>
    {
        public async Task Handle(RegisterOutgoingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterOutgoing(command.Quantity);
            await repo.UpdateAsync(item, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
}
