using Warehouse.Application.UseCases.Inventory.Dtos;

namespace Warehouse.Application.Ports.Inventory
{
    public interface IInventoryReadRepository
    {
        Task<ItemDetailsDto?> GetDetailsAsync(string sku, CancellationToken ct = default);
        Task<IReadOnlyList<ItemListDto>> ListAsync(CancellationToken ct = default);
    }
}
