namespace OrderProcessor.Domain;

public class Product
{
    public string Name { get; init; } = string.Empty;
    public Color Color { get; init; }
    public Size Size { get; init; }
    public Pricing Pricing { get; init; }
    public Material Material { get; init; }
    public decimal Weight { get; init; }
    public bool Fragility { get; init; }
    public bool ContainsLiquids { get; init; }
    public Packaging Packaging { get; init; }
    public Dimensions Dimensions { get; init; }
}
