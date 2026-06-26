using OrderProcessor.Domain;

namespace OrderProcessor.Tests.Domain;

public class OrderConfirmationTests
{
    [Fact]
    public void ConfirmingPendingOrder_ChangesStatusToConfirmed()
    {
        var order = new Order();

        order.Confirm();

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void ConfirmingAlreadyConfirmedOrder_Throws()
    {
        var order = new Order();
        order.Confirm();

        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void ConfirmingCancelledOrder_Throws()
    {
        var order = new Order();
        order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }
}
