using OrderProcessor.Domain;

namespace OrderProcessor.Tests.Domain;

public class OrderTests
{
    [Fact]
    public void OrderTotalIsProductPriceAfterDiscount()
    {
        var order = new Order();
        order.AddProduct(new Product { Pricing = new Pricing(new Money(10.00m), new Discount(0.10m)) });

        Assert.Equal(9.00m, order.Total);
    }

    [Fact]
    public void OrderTotalIsRoundedToTwoDecimalPlaces()
    {
        var order = new Order();
        order.AddProduct(new Product { Pricing = new Pricing(new Money(9.99m), new Discount(0.1m)) });

        Assert.Equal(8.99m, order.Total);
    }
}
