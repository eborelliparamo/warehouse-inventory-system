using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Warehouse.Audit.Infrastructure;

namespace Warehouse.Audit.Worker;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _sp;
    private readonly IConnection _conn;
    private readonly string _exchange;
    private readonly string _queue;
    private readonly string _routingPattern;
    private readonly ushort _prefetch;

    public Worker(ILogger<Worker> logger, IServiceProvider sp, IConnection conn, IConfiguration cfg)
    {
        _logger = logger;
        _sp = sp;
        _conn = conn;
        _exchange = cfg["Rabbit:Exchange"] ?? "inventory";
        _queue = cfg["Rabbit:Queue"] ?? "audit.inventory";
        _routingPattern = cfg["Rabbit:RoutingPattern"] ?? "inventory.*";
        _prefetch = ushort.TryParse(cfg["Rabbit:Prefetch"], out var p) ? p : (ushort)10;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var ch = _conn.CreateModel();
        ch.ExchangeDeclare("inventory", ExchangeType.Topic, durable: true);
        ch.QueueDeclare("audit.inventory", durable: true, exclusive: false, autoDelete: false, arguments: null);
        ch.QueueBind("audit.inventory", "inventory", "inventory.*");

        ch.BasicQos(0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(ch);
        consumer.Received += async (_, ea) =>
        {
            _logger.LogInformation("Recibido tag={Tag} len={Len} rk={RK}", ea.DeliveryTag, ea.Body.Length, ea.RoutingKey);
            try
            {
                var env = Warehouse.Contracts.Inventory.InventoryEventEnvelope.Parser.ParseFrom(ea.Body.Span);

                string sku; int delta;
                switch (env.EventType)
                {
                    case "ItemCreated": sku = env.ItemCreated.Sku; delta = 0; break;
                    case "StockInRegistered": sku = env.StockInRegistered.Sku; delta = env.StockInRegistered.Quantity; break;
                    case "StockOutRegistered": sku = env.StockOutRegistered.Sku; delta = -env.StockOutRegistered.Quantity; break;
                    default:
                        _logger.LogWarning("Evento desconocido: {Type}", env.EventType);
                        ch.BasicAck(ea.DeliveryTag, false);
                        return;
                }

                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
                db.AuditEvents.Add(new AuditEvent
                {
                    Id = Guid.NewGuid(),
                    StreamId = Guid.Parse(env.StreamId),
                    Version = env.Version,
                    Sku = sku,
                    Type = env.EventType,
                    Delta = delta,
                    OccurredAt = env.OccurredAt.ToDateTime()
                });

                await db.SaveChangesAsync(ct);
                ch.BasicAck(ea.DeliveryTag, false);
                _logger.LogInformation("ACK tag={Tag}", ea.DeliveryTag);
            }
            catch (DbUpdateException)
            {
                ch.BasicAck(ea.DeliveryTag, false);
                _logger.LogInformation("Duplicado, ACK tag={Tag}", ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje tag={Tag}");
                ch.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        consumer.Shutdown += (_, args) => { _logger.LogWarning("Consumer shutdown: {Cause}", args.Cause); return Task.CompletedTask; };
        consumer.Registered += (_, __) => { _logger.LogInformation("Consumer registrado"); return Task.CompletedTask; };
        consumer.Unregistered += (_, __) => { _logger.LogWarning("Consumer unregistrado"); return Task.CompletedTask; };
        consumer.ConsumerCancelled += (_, __) => { _logger.LogWarning("Consumer cancelado"); return Task.CompletedTask; };

        ch.BasicConsume(queue: "audit.inventory", autoAck: false, consumer: consumer);
        _logger.LogInformation("Worker escuchando 'audit.inventory'");

        while (!ct.IsCancellationRequested) await Task.Delay(1000, ct);

        try { ch?.Close(); ch?.Dispose(); } catch { }
    }
}