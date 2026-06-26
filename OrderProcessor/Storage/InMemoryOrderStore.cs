using OrderProcessor.Domain;

namespace OrderProcessor.Storage;

public class InMemoryOrderStore : IOrderStore
{
    private readonly List<Order> _orders = new();

    public void Add(Order order) => _orders.Add(order);

    public void Update(Order order) { }

    public IReadOnlyList<Order> All() => _orders.AsReadOnly();

    public Order? Find(Guid id) => _orders.FirstOrDefault(o => o.Id == id);
}
