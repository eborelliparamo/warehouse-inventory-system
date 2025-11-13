namespace Warehouse.Infrastructure.EventSourcing
{
    public sealed record ItemCreatedDto(string sku, string name);
    public sealed record StockInRegisteredDto(string sku, int quantity);
    public sealed record StockOutRegisteredDto(string sku, int quantity);
}
