using Warehouse.Domain.ValueObjects;

namespace Warehouse.Domain.Events
{
    public sealed record ItemCreated(Sku Sku, string Name, DateTime OccurredAtUtc) : IDomainEvent;
    public sealed record StockInRegistered(Sku Sku, MovementQuantity Quantity, DateTime OccurredAtUtc) : IDomainEvent;
    public sealed record StockOutRegistered(Sku Sku, MovementQuantity Quantity, DateTime OccurredAtUtc) : IDomainEvent;
}
