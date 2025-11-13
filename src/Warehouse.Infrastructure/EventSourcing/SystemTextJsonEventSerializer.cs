using System.Text.Json;
using Warehouse.Domain.Events;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Infrastructure.EventSourcing
{
    public sealed class SystemTextJsonEventSerializer : IEventSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public (string type, JsonDocument data, JsonDocument metadata, DateTime when) Serialize(IDomainEvent e)
        {
            var when = DateTime.UtcNow;

            string type = e.GetType().Name;
            byte[] bytes = e switch
            {
                ItemCreated x
                    => JsonSerializer.SerializeToUtf8Bytes(new ItemCreatedDto(x.Sku.Value, x.Name), Options),

                StockInRegistered x
                    => JsonSerializer.SerializeToUtf8Bytes(new StockInRegisteredDto(x.Sku.Value, x.Quantity.Value), Options),

                StockOutRegistered x
                    => JsonSerializer.SerializeToUtf8Bytes(new StockOutRegisteredDto(x.Sku.Value, x.Quantity.Value), Options),

                _ => throw new NotSupportedException($"Unknown event {e.GetType().Name}")
            };

            var data = JsonDocument.Parse(bytes);
            var meta = JsonDocument.Parse("""{"schema":"v1"}""");

            return (type, data, meta, when);
        }

        public T Deserialize<T>(JsonDocument data)
            => JsonSerializer.Deserialize<T>(data.RootElement.GetRawText(), Options)!;

        public IDomainEvent DeserializeByName(string type, JsonDocument data)
            => type switch
            {
                nameof(ItemCreated) =>
                    Map(Read<ItemCreatedDto>(data), d => new ItemCreated(new Sku(d.sku), d.name, DateTime.UtcNow)),

                nameof(StockInRegistered) =>
                    Map(Read<StockInRegisteredDto>(data), d => new StockInRegistered(new Sku(d.sku), MovementQuantity.Create(d.quantity), DateTime.UtcNow)),

                nameof(StockOutRegistered) =>
                    Map(Read<StockOutRegisteredDto>(data), d => new StockOutRegistered(new Sku(d.sku), MovementQuantity.Create(d.quantity), DateTime.UtcNow)),

                _ => throw new NotSupportedException($"Unknown event type '{type}'")
            };

        private static TDto Read<TDto>(JsonDocument doc)
            => JsonSerializer.Deserialize<TDto>(doc.RootElement.GetRawText(), Options)!;

        private static IDomainEvent Map<TDto>(TDto dto, Func<TDto, IDomainEvent> projector)
            => projector(dto);
    }
}
