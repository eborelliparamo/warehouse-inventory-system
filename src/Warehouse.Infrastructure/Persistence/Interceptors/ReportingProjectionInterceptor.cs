using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Warehouse.Domain.Events;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.ReadModel;

namespace Warehouse.Infrastructure.Persistence.Interceptors
{
    public sealed class PostCommitReadModelInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDomainEventCollector _events;
        private readonly ILogger<PostCommitReadModelInterceptor> _logger;

        public PostCommitReadModelInterceptor(
            IServiceScopeFactory scopeFactory,
            IDomainEventCollector events,
            ILogger<PostCommitReadModelInterceptor> logger)
            => (_scopeFactory, _events, _logger) = (scopeFactory, events, logger);

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData e, int result, CancellationToken ct = default)
        {
            if (e.Context is not WarehouseWriteDbContext) return result;

            var drained = _events.Drain();
            if (drained.Count == 0) return result;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var readDb = scope.ServiceProvider.GetRequiredService<WarehouseReadDbContext>();
                var projectors = scope.ServiceProvider.GetRequiredService<IEventProjectors>();

                foreach (var evt in drained)
                    if (projectors.TryGet(evt.GetType(), out var handler))
                        await handler(readDb, evt, ct);

                await readDb.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Read model projection failed post-commit");
            }
            return result;
        }
    }
}
