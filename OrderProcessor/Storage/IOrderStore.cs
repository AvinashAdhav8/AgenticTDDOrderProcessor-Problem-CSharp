using OrderProcessor.Domain;

namespace OrderProcessor.Storage;

public interface IOrderStore
{
    void Add(Order order);
    void Update(Order order);
    IReadOnlyList<Order> All();
    Order? Find(Guid id);
}
