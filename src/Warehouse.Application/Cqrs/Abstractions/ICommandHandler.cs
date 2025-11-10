namespace Warehouse.Application.Cqrs.Abstractions
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command, CancellationToken ct);
    }
}
