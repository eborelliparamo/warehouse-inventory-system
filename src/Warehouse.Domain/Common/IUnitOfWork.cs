namespace Warehouse.Domain.Common
{
    public interface IUnitOfWork { Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct); }
}
