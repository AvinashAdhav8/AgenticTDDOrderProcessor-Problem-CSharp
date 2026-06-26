using OrderProcessor.Domain;
using OrderProcessor.Storage;

namespace OrderProcessor.Tests.Storage;

public class JsonOrderStoreTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"orders-{Guid.NewGuid():N}.json");

    private static Product SampleProduct() => new()
    {
        Name = "Wine Glass",
        Color = Color.Red,
        Size = Size.Small,
        Pricing = new Pricing(new Money(10.00m), new Discount(0.10m)),
        Material = Material.Glass,
        Weight = 0.3m,
        Fragility = true,
        ContainsLiquids = false,
        Packaging = Packaging.Boxed,
        Dimensions = new Dimensions(8.0m, 8.0m, 20.0m),
    };

    [Fact]
    public void OrdersWithProductsPersistAcrossInstances()
    {
        var order = new Order();
        order.AddProduct(SampleProduct());

        var writer = new JsonOrderStore(_path);
        writer.Add(order);

        var reader = new JsonOrderStore(_path);
        var loaded = reader.Find(order.Id);

        Assert.NotNull(loaded);
        Assert.Equal(9.00m, loaded!.Total);
    }

    [Fact]
    public void ProductsAddedAfterStoringPersistOnUpdate()
    {
        var store = new JsonOrderStore(_path);
        var order = new Order();
        store.Add(order);

        order.AddProduct(SampleProduct());
        store.Update(order);

        var reloaded = new JsonOrderStore(_path).Find(order.Id);
        Assert.Equal(9.00m, reloaded!.Total);
    }

    [Fact]
    public void ConfirmedOrderStatusSurvivesRestart()
    {
        var order = new Order();
        order.Confirm();

        var writer = new JsonOrderStore(_path);
        writer.Add(order);

        var reader = new JsonOrderStore(_path);
        var loaded = reader.Find(order.Id);

        Assert.Equal(OrderStatus.Confirmed, loaded!.Status);
    }

    public void Dispose()
    {
        if (File.Exists(_path))
            File.Delete(_path);
    }
}
