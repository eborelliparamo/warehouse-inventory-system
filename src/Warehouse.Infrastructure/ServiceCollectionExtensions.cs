using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Warehouse.Domain.Abstractions.Inventory;
using Warehouse.Domain.Common;
using Warehouse.Domain.Events;
using Warehouse.Domain.Inventory;
using Warehouse.Infrastructure.EventSourcing;
using Warehouse.Infrastructure.Outbox;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Persistence.Data;
using Warehouse.Infrastructure.Persistence.Interceptors;
using Warehouse.Infrastructure.ReadModel.Projectors;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureWrite(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddScoped<OutboxSaveChangesInterceptor>();

            var cs = cfg.GetConnectionString("Warehouse")
                     ?? cfg["ConnectionStrings:Warehouse"]; 
            var host = cfg["Rabbit:Host"] ?? "localhost";     

            services.AddDbContext<WarehouseWriteDbContext>((sp, o) => o.UseNpgsql(cs).UseSnakeCaseNamingConvention().AddInterceptors(sp.GetRequiredService<OutboxSaveChangesInterceptor>()));
            services.AddDbContext<WarehouseReadDbContext>(o => o.UseNpgsql(cs).UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddSingleton<RabbitMQ.Client.IConnection>(_ => new ConnectionFactory { HostName = host }.CreateConnection());

            services.AddScoped<IEventSerializer, SystemTextJsonEventSerializer>();
            services.AddScoped<IEventStore, EfEventStore>();
            services.AddScoped<IInventoryEventSourcedRepository, InventoryEventSourcedRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEventProjector, ItemCreatedProjector>();
            services.AddScoped<IEventProjector, StockInProjector>();
            services.AddScoped<IEventProjector, StockOutProjector>();
            services.AddScoped<IInventoryReadRepository, InventoryRepository>();
            services.AddHostedService<OutboxDispatcher>();

            return services;
        }

        public static IServiceCollection AddInfrastructureRead(this IServiceCollection services, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("Warehouse")
                     ?? cfg["ConnectionStrings:Warehouse"]
                     ?? throw new InvalidOperationException("Missing connection string 'Warehouse'.");
            services.AddDbContext<WarehouseReadDbContext>(o => o.UseNpgsql(cs).UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            return services;
        }
    }
}
