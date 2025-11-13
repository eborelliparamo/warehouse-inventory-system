using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Domain.Common;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock
{
    public sealed class RegisterOutgoingStockHandler(IInventoryEventSourcedRepository repo, IUnitOfWork uow)
        : ICommandHandler<RegisterOutgoingStockCommand>
    {
        public async Task Handle(RegisterOutgoingStockCommand cmd, CancellationToken ct)
        {
            await uow.ExecuteAsync(async _ =>
            {
                var agg = await repo.GetAsync(cmd.Sku, ct);
                agg.StockOut(cmd.Quantity);
                await repo.SaveAsync(agg, ct);
            }, ct);
        }
    }
}
