using Microsoft.AspNetCore.Http.HttpResults;
using Warehouse.Api.Filters;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.CreateItem;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Application.UseCases.Inventory.GetItemBySku;
using Warehouse.Application.UseCases.Inventory.ListItems;
using Warehouse.Application.UseCases.Inventory.RegisterIncomingStock;
using Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Api.Modules
{
    public static class InventoryModule
    {
        public static IEndpointRouteBuilder MapInventoryModule(this IEndpointRouteBuilder app)
        {
            var grp = app.MapGroup("/items").WithTags("Inventory");

            grp.MapPost("", async (CreateItemCommand req, IProcessor bus, WarehouseDbContext db) =>
            {
                await bus.Send(req);
                await db.SaveChangesAsync();
                return Results.Created($"/items/{req.Sku}", null);
            }).AddEndpointFilter(new ValidationFilter<CreateItemCommand>());


            grp.MapPost("/{sku}/stock-in", async (string sku, RegisterIncomingStockCommand body, IProcessor bus, WarehouseDbContext db) =>
            {
                var cmd = body with { Sku = sku };
                await bus.Send(cmd);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).AddEndpointFilter(new ValidationFilter<RegisterIncomingStockCommand>());


            grp.MapPost("/{sku}/stock-out", async (string sku, RegisterOutgoingStockCommand body, IProcessor bus, WarehouseDbContext db) =>
            {
                var cmd = body with { Sku = sku };
                await bus.Send(cmd);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).AddEndpointFilter(new ValidationFilter<RegisterOutgoingStockCommand>());


            grp.MapGet("/{sku}", async Task<Results<Ok<ItemDetailsDto>, NotFound>> (string sku, IProcessor bus) =>
            {
                var dto = await bus.Query<GetItemBySkuQuery, ItemDetailsDto?>(new(sku));
                return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
            });


            grp.MapGet("", async (IProcessor bus) =>
            {
                var list = await bus.Query<ListItemsQuery, IReadOnlyList<ItemListDto>>(new());
                return Results.Ok(list);
            });

            return app;
        }
    }
}
