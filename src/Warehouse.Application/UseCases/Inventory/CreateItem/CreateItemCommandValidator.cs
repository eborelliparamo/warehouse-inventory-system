using FluentValidation;

namespace Warehouse.Application.UseCases.Inventory.CreateItem
{
    public sealed class Validator : AbstractValidator<CreateItemCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Sku.Value).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
