using Warehouse.Domain.Common;

namespace Warehouse.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        public Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct) => action(ct);
    }
}
