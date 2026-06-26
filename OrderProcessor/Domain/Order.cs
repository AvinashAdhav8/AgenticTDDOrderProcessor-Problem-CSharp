namespace OrderProcessor.Domain;

public class Order
{
    private readonly List<Product> _products = new();

    public Guid Id { get; } = Guid.NewGuid();
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public Order() { }

    public Order(Guid id, OrderStatus status = OrderStatus.Pending) { Id = id; Status = status; }

    public void AddProduct(Product product) => _products.Add(product);

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm an order with status {Status}.");
        Status = OrderStatus.Confirmed;
    }

    public void Cancel() => Status = OrderStatus.Cancelled;

    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public decimal Total => Math.Round(_products.Sum(p => p.DiscountedPrice), 2, MidpointRounding.AwayFromZero);
}
