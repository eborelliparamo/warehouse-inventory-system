namespace Warehouse.Domain.ValueObjects
{
    public sealed record InventoryQuantity
    {
        public int Value { get; }
        public static readonly InventoryQuantity Zero = new(0);
        public InventoryQuantity(int value) => Value = value;

        public static implicit operator int(InventoryQuantity q) => q.Value;
        public static implicit operator InventoryQuantity(int value) => new(value);

        public static InventoryQuantity operator +(InventoryQuantity total, MovementQuantity delta)
        {
            ArgumentNullException.ThrowIfNull(total);
            ArgumentNullException.ThrowIfNull(delta);
            return new(checked(total.Value + delta.Value));
        }

        public static InventoryQuantity operator -(InventoryQuantity total, MovementQuantity delta)
        {
            ArgumentNullException.ThrowIfNull(total);
            ArgumentNullException.ThrowIfNull(delta);
            return new(checked(total.Value - delta.Value));
        }
    }
}
