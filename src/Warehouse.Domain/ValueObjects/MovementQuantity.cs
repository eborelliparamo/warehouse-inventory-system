namespace Warehouse.Domain.ValueObjects
{
    public sealed record MovementQuantity
    {
        public int Value { get; }
        private MovementQuantity(int value) => Value = value;

        public static MovementQuantity Create(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Movement quantity must be > 0.");
            return new(quantity);
        }

        public static implicit operator MovementQuantity(int quantity) => Create(quantity);
    }
}
