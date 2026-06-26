using System.Text.Json.Serialization;

namespace OrderProcessor.Domain;

public readonly record struct Discount
{
    public decimal Value { get; }

    [JsonConstructor]
    public Discount(decimal value)
    {
        if (value < 0m || value > 1m)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Discount must be between 0 and 1.");
        Value = value;
    }
}
