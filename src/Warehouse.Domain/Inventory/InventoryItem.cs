using Warehouse.Domain.Events;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Domain.Inventory
{
    public sealed class InventoryItem : Entity
    {
        public Guid Id { get; private set; }
        public Sku Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public InventoryQuantity TotalQuantity { get; private set; } = InventoryQuantity.Zero;

        private InventoryItem() { }

        public static InventoryItem CreateOrThrow(Sku sku, string name)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            var item = new InventoryItem { Id = Guid.NewGuid(), Sku = sku, Name = name.Trim(), TotalQuantity = InventoryQuantity.Zero };
            item.Raise(new ItemCreated(sku, name, DateTime.UtcNow));

            return item;
        }

        public void RegisterIncoming(MovementQuantity qty)
        {
            TotalQuantity += qty;
            Raise(new StockInRegistered(Sku, qty, DateTime.UtcNow));
        }
        public void RegisterOutgoing(MovementQuantity qty)
        {
            TotalQuantity -= qty;
            Raise(new StockOutRegistered(Sku, qty, DateTime.UtcNow));
        }
    }
}
