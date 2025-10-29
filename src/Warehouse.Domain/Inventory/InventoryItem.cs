namespace Warehouse.Domain.Inventory
{
    public sealed class InventoryItem
    {
        public Guid Id { get; private set; }
        public string Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public int TotalQuantity { get; private set; }

        private InventoryItem() { }


        public static InventoryItem Create(string sku, string name)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            return new InventoryItem { Id = Guid.NewGuid(), Sku = sku.Trim(), Name = name.Trim(), TotalQuantity = 0 };
        }

        public void RegisterIncoming(int qty) { if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty)); TotalQuantity += qty; }
        public void RegisterOutgoing(int qty) { if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty)); TotalQuantity -= qty; }
    }
}
