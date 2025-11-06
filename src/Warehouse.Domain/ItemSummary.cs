namespace Warehouse.Domain
{
    public sealed class ItemSummary
    {
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; } = 10;
    }
}
