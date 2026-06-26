using System.Text.Json.Serialization;

namespace OrderProcessor.Domain;

public readonly record struct Dimensions
{
    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }

    [JsonConstructor]
    public Dimensions(decimal length, decimal width, decimal height)
    {
        Length = length;
        Width = width;
        Height = height;
    }
}
