using OrderProcessor.Domain;

namespace OrderProcessor.Storage;

public record OrderRecord(Guid Id, List<Product> Products, OrderStatus Status = OrderStatus.Pending)
{
    public static OrderRecord From(Order order) =>
        new(order.Id, order.Products.ToList(), order.Status);

    public Order ToOrder()
    {
        var order = new Order(Id, Status);
        foreach (var product in Products)
            order.AddProduct(product);
        return order;
    }
}
