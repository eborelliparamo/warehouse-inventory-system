using System.ComponentModel.DataAnnotations;

namespace Warehouse.Api.Dtos
{
    public sealed record ChangeStockRequest(
        [property: Range(1, int.MaxValue, ErrorMessage = "Quantity debe ser > 0")] int Quantity
    );
}
