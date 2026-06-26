using OrderProcessor.Domain;

namespace OrderProcessor.Tests.Domain;

public class DiscountTests
{
    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void RejectsValuesOutsideZeroToOne(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Discount((decimal)value));
    }
}
