using OrderProcessor.Domain;

namespace OrderProcessor.Storage;

public record OrderRecord(Guid Id, List<Product> Products)
{
    public static OrderRecord From(Order order) =>
        new(order.Id, order.Products.ToList());

    public Order ToOrder()
    {
        var order = new Order(Id);
        foreach (var product in Products)
            order.AddProduct(product);
        return order;
    }
}
