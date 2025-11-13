using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Common;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.RegisterIncomingStock
{
    public sealed class RegisterIncomingStockHandler(IInventoryEventSourcedRepository repo, IUnitOfWork uow) : ICommandHandler<RegisterIncomingStockCommand>
    {
        public async Task Handle(RegisterIncomingStockCommand command, CancellationToken ct)
        {
            await uow.ExecuteAsync(async _ =>
            {
                var agg = await repo.GetAsync(command.Sku, ct);
                agg.StockIn(command.Quantity);
                await repo.SaveAsync(agg, ct);
            }, ct);
        }
    }
}
