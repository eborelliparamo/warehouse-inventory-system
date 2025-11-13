using Warehouse.Domain.Common;
using Warehouse.Domain.Events;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Domain.Inventory
{
    public sealed class InventoryItemAggregate : AggregateRoot
    {
        public Sku Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public int Balance { get; private set; }

        public static InventoryItemAggregate CreateNew(Sku sku, string name, Guid streamId)
        {
            var a = new InventoryItemAggregate { StreamId = streamId };
            a.Raise(new ItemCreated(sku, name, DateTime.UtcNow));
            return a;
        }

        public void StockIn(MovementQuantity qty) => Raise(new StockInRegistered(Sku, qty, DateTime.UtcNow));

        public void StockOut(MovementQuantity qty)
        {
            if (Balance - qty.Value < 0) throw new InvalidOperationException("Negative stock not allowed.");
            Raise(new StockOutRegistered(Sku, qty, DateTime.UtcNow));
        }

        protected override void When(IDomainEvent e)
        {
            switch (e)
            {
                case ItemCreated x: Sku = x.Sku; Name = x.Name; break;
                case StockInRegistered x: Balance += x.Quantity.Value; break;
                case StockOutRegistered x: Balance -= x.Quantity.Value; break;
            }
        }
    }
}
