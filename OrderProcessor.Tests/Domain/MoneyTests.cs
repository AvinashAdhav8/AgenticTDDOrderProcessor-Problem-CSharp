using OrderProcessor.Domain;

namespace OrderProcessor.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void RejectsNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-0.01m));
    }
}
