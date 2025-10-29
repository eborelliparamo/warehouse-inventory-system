using FluentValidation;
using Warehouse.Application.Cqrs.Abstractions;
using Warehouse.Application.Ports.Inventory;
using Warehouse.Domain.Inventory;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed record CreateItemCommand(string Sku, string Name) : ICommand;

    public sealed class Handler(IInventoryWriteRepository repo) : ICommandHandler<CreateItemCommand>
    {
        public async Task Handle(CreateItemCommand command, CancellationToken ct)
        {
            var existing = await repo.GetBySkuAsync(command.Sku, ct);
            if (existing is not null) throw new InvalidOperationException($"SKU '{command.Sku}' already exists.");
            var item = InventoryItem.Create(command.Sku, command.Name);
            await repo.AddAsync(item, ct);
        }
    }

    public sealed class Validator : AbstractValidator<CreateItemCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
