namespace Warehouse.Domain.ValueObjects
{
    public readonly record struct Sku
    {
        public string Value { get; }

        public Sku(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU can not be empty", nameof(value));

            var v = value.Trim().ToUpperInvariant();

            Value = v;
        }

        public override string ToString() => Value;

        public static implicit operator string(Sku s) => s.Value;
        public static explicit operator Sku(string s) => new(s);
    }
}
