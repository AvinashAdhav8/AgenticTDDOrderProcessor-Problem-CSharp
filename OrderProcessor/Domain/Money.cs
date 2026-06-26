using System.Text.Json.Serialization;

namespace OrderProcessor.Domain;

public readonly record struct Money
{
    public decimal Amount { get; }

    [JsonConstructor]
    public Money(decimal amount)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Money cannot be negative.");
        Amount = amount;
    }
}
