namespace OrderProcessor.Domain;

public class Order
{
    private readonly List<Product> _products = new();

    public Guid Id { get; } = Guid.NewGuid();

    public Order() { }

    public Order(Guid id) => Id = id;

    public void AddProduct(Product product) => _products.Add(product);

    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public decimal Total => Math.Round(_products.Sum(p => p.Pricing.DiscountedPrice), 2, MidpointRounding.AwayFromZero);
}
