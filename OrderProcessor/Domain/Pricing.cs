using System.Text.Json.Serialization;

namespace OrderProcessor.Domain;

public readonly record struct Pricing
{
    public Money Price { get; }
    public Discount Discount { get; }

    [JsonConstructor]
    public Pricing(Money price, Discount discount)
    {
        Price = price;
        Discount = discount;
    }

    public static Pricing Of(decimal price, decimal discount) =>
        new(new Money(price), new Discount(discount));

    public decimal DiscountedPrice => Price.Amount * (1 - Discount.Value);
}
