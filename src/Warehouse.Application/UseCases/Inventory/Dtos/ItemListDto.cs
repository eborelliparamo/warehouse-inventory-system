namespace Warehouse.Application.UseCases.Inventory.Dtos
{
    public sealed record ItemListDto(string Sku, string Name, int TotalQuantity);
}
