using System.Text.Json;

namespace Warehouse.Domain.Events
{
    public interface IEventSerializer
    {
        (string type, JsonDocument data, JsonDocument metadata, DateTime when) Serialize(IDomainEvent e);
        T Deserialize<T>(JsonDocument data);
        IDomainEvent DeserializeByName(string type, JsonDocument data);
    }
}
