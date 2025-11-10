using Warehouse.Api.Dtos;
using Warehouse.Api.Filters;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.UseCases.Inventory.CreateItem;
using Warehouse.Application.UseCases.Inventory.Dtos;
using Warehouse.Application.UseCases.Inventory.GetItemBySku;
using Warehouse.Application.UseCases.Inventory.ListItems;
using Warehouse.Application.UseCases.Inventory.RegisterIncomingStock;
using Warehouse.Application.UseCases.Inventory.RegisterOutgoingStock;
using Warehouse.Domain.ValueObjects;

namespace Warehouse.Api.Modules
{
    public static class InventoryModule
    {
        public static IEndpointRouteBuilder MapInventoryModule(this IEndpointRouteBuilder app)
        {
            var grp = app.MapGroup("/items").WithTags("Inventory");

            grp.MapPost("", async (CreateItemRequest body,
                                   ICommandHandler<CreateItemCommand> handler,
                                   CancellationToken ct) =>
            {
                await handler.Handle(new CreateItemCommand(new Sku(body.Sku), body.Name), ct);
                return Results.Created($"/items/{body.Sku}", null);
            });

            grp.MapPost("/{sku}/stock-in", async (string sku, ChangeStockRequest body, ICommandHandler<RegisterIncomingStockCommand> handler, CancellationToken ct) =>
            {
                await handler.Handle(new RegisterIncomingStockCommand(new Sku(sku), (MovementQuantity)body.Quantity), ct);
                return Results.NoContent();
            }).AddEndpointFilter(new ValidationFilter<RegisterIncomingStockCommand>());

            grp.MapPost("/{sku}/stock-out", async (string sku, ChangeStockRequest body, ICommandHandler<RegisterOutgoingStockCommand> handler, CancellationToken ct) =>
            {
                await handler.Handle(new RegisterOutgoingStockCommand(new Sku(sku), (MovementQuantity)body.Quantity), ct);
                return Results.NoContent();
            }).AddEndpointFilter(new ValidationFilter<RegisterOutgoingStockCommand>());

            grp.MapGet("/{sku}", async (
                string sku,
                IQueryHandler<GetItemBySkuQuery, ItemDetailsDto?> handler,
                CancellationToken ct) =>
            {
                var result = await handler.Handle(new GetItemBySkuQuery(new Sku(sku)), ct);
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetItemBySku");

            grp.MapGet("", async (
                IQueryHandler<ListItemsQuery, IReadOnlyList<ItemListDto>> handler,
                CancellationToken ct) =>
            {
                var list = await handler.Handle(new ListItemsQuery(), ct);
                return Results.Ok(list);
            })
            .WithName("ListItems");

            return app;
        }
    }
}
