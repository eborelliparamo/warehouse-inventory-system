using Microsoft.Extensions.DependencyInjection;
using Warehouse.Application.Cqrs.Abstractions;

namespace Warehouse.Application.Cqrs
{
    public sealed class Processor(IServiceProvider sp) : IProcessor
    {
        public Task Send<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
        {
            var handler = sp.GetRequiredService<ICommandHandler<TCommand>>();
            return handler.Handle(command, ct);
        }


        public Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResult>
        {
            var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return handler.Handle(query, ct);
        }
    }
}
