namespace Warehouse.Application.Cqrs.Abstractions
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query, CancellationToken ct);
    }
}
