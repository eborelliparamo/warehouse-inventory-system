namespace Warehouse.Application.Cqrs.Abstractions
{
    public interface IProcessor
    {
        Task Send<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand;
        Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResult>;
    }
}
