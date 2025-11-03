using System.ComponentModel.DataAnnotations;

namespace Warehouse.Api.Dtos
{
    public sealed record CreateItemRequest(
        [property: Required, StringLength(64)] string Sku,
        [property: Required, StringLength(200)] string Name
    );
}
