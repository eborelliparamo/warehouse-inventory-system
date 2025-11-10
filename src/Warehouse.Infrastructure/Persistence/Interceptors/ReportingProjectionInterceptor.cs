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
        private readonly IHaveDomainEvents _events;
        private readonly ILogger<PostCommitReadModelInterceptor> _logger;

        public PostCommitReadModelInterceptor(
            IServiceScopeFactory scopeFactory,
            IHaveDomainEvents events,
            ILogger<PostCommitReadModelInterceptor> logger)
            => (_scopeFactory, _events, _logger) = (scopeFactory, events, logger);

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData e, int result, CancellationToken ct = default)
        {
            if (e.Context is not WarehouseWriteDbContext writeDb) return result;

            var domainEvents = writeDb.ChangeTracker
                .Entries<IHaveDomainEvents>()
                .SelectMany(en => en.Entity.DomainEvents)
                .ToArray();

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var readDb = scope.ServiceProvider.GetRequiredService<WarehouseReadDbContext>();
                var projectors = scope.ServiceProvider.GetRequiredService<IEventProjectors>();

                foreach (var evt in domainEvents)
                    if (projectors.TryGet(evt.GetType(), out var handler))
                        await handler(readDb, evt, ct);

                await readDb.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Read model projection failed post-commit");
            }
            finally
            {
                foreach (var entry in writeDb.ChangeTracker.Entries<IHaveDomainEvents>())
                    entry.Entity.ClearDomainEvents();
            }
            return result;
        }
    }
}
