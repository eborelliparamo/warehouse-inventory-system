namespace Warehouse.Application.UseCases.Inventory.Dtos
{
    public sealed record ItemDetailsDto(string Sku, string Name, int TotalQuantity);
}
