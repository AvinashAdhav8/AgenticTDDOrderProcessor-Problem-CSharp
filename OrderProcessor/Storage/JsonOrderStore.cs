using System.Text.Json;
using System.Text.Json.Serialization;
using OrderProcessor.Domain;

namespace OrderProcessor.Storage;

public class JsonOrderStore : IOrderStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly string _path;
    private readonly List<Order> _orders;

    public JsonOrderStore(string path)
    {
        _path = path;
        _orders = Load();
    }

    public void Add(Order order)
    {
        _orders.Add(order);
        Persist();
    }

    public void Update(Order order) => Persist();

    public IReadOnlyList<Order> All() => _orders.AsReadOnly();

    public Order? Find(Guid id) => _orders.FirstOrDefault(o => o.Id == id);

    private List<Order> Load()
    {
        if (!File.Exists(_path))
            return new List<Order>();

        var json = File.ReadAllText(_path);
        var records = JsonSerializer.Deserialize<List<OrderRecord>>(json, Options) ?? new List<OrderRecord>();
        return records.Select(r => r.ToOrder()).ToList();
    }

    private void Persist()
    {
        var records = _orders.Select(OrderRecord.From).ToList();
        File.WriteAllText(_path, JsonSerializer.Serialize(records, Options));
    }
}
