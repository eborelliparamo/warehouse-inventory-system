using Warehouse.Application.Abstractions;
using Warehouse.Application.Abstractions.Inventory;
using Warehouse.Application.Cqrs.Abstractions;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed class RegisterIncomingStockHandler(IInventoryWriteRepository repo, IUnitOfWork uow) : ICommandHandler<RegisterIncomingStockCommand>
    {
        public async Task Handle(RegisterIncomingStockCommand command, CancellationToken ct)
        {
            var item = await repo.GetBySkuAsync(command.Sku, ct) ?? throw new KeyNotFoundException($"SKU '{command.Sku}' not found.");
            item.RegisterIncoming(command.Quantity);
            await repo.UpdateAsync(item, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
}
