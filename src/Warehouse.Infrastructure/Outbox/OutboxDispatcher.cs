using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Warehouse.Infrastructure.Persistence.Data;

namespace Warehouse.Infrastructure.Outbox
{
    public sealed class OutboxDispatcher : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly IConnection _conn;
        private readonly ILogger<OutboxDispatcher> _logger;

        private const string Exchange = "inventory";

        public OutboxDispatcher(IServiceProvider sp, IConnection conn, ILogger<OutboxDispatcher> logger)
            => (_sp, _conn, _logger) = (sp, conn, logger);

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<WarehouseWriteDbContext>();

                    var batch = await db.Outbox
                        .Where(x => x.PublishedAt == null)
                        .OrderBy(x => x.CreatedAt)
                        .Take(100)
                        .ToListAsync(ct);

                    if (batch.Count == 0) { await Task.Delay(1000, ct); continue; }

                    using var ch = _conn.CreateModel();
                    ch.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true);

                    foreach (var row in batch)
                    {
                        var props = ch.CreateBasicProperties();
                        props.ContentType = row.ContentType;
                        props.DeliveryMode = 2;

                        var routingKey = $"inventory.{row.Kind}";
                        ch.BasicPublish(
                            exchange: Exchange,
                            routingKey: routingKey,
                            mandatory: true,
                            basicProperties: props,
                            body: row.Payload);

                        row.PublishedAt = DateTime.UtcNow;
                        row.Attempts += 1;

                        _logger.LogInformation("Published {Kind} v{Version} to {RoutingKey}", row.Kind, row.Version, routingKey);
                    }

                    await db.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OutboxDispatcher error");
                    await Task.Delay(2000, ct);
                }
            }
        }
    }
}
