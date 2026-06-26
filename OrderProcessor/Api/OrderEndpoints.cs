using OrderProcessor.Domain;
using OrderProcessor.Storage;

namespace OrderProcessor.Api;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/api/orders", (IOrderStore store) =>
        {
            var order = new Order();
            store.Add(order);
            return Results.Created($"/api/orders/{order.Id}", ToResponse(order));
        });

        app.MapGet("/api/orders", (IOrderStore store) =>
            Results.Ok(store.All().Select(ToResponse)));

        app.MapPost("/api/orders/{id:guid}/products", (Guid id, AddProductRequest request, IOrderStore store) =>
        {
            var order = store.Find(id);
            if (order is null)
                return Results.NotFound();

            order.AddProduct(request.ToProduct());
            store.Update(order);
            return Results.Created($"/api/orders/{order.Id}", ToResponse(order));
        });
    }

    private static object ToResponse(Order o) => new
    {
        o.Id,
        o.Total,
        Products = o.Products.Select(p => new
        {
            p.Name,
            p.Color,
            p.Size,
            Price = p.Pricing.Price.Amount,
            Discount = p.Pricing.Discount.Value,
            p.Material,
            p.Weight,
            p.Fragility,
            p.ContainsLiquids,
            p.Packaging,
            Dimensions = new { p.Dimensions.Length, p.Dimensions.Width, p.Dimensions.Height },
        }),
    };

    private record AddProductRequest(
        string Name, Color Color, Size Size, decimal Price, decimal Discount,
        Material Material, decimal Weight, bool Fragility, bool ContainsLiquids,
        Packaging Packaging, DimensionsRequest? Dimensions)
    {
        public Product ToProduct() => new()
        {
            Name = Name,
            Color = Color,
            Size = Size,
            Pricing = Pricing.Of(Price, Discount),
            Material = Material,
            Weight = Weight,
            Fragility = Fragility,
            ContainsLiquids = ContainsLiquids,
            Packaging = Packaging,
            Dimensions = Dimensions is null
                ? default
                : new Dimensions(Dimensions.Length, Dimensions.Width, Dimensions.Height),
        };
    }

    private record DimensionsRequest(decimal Length, decimal Width, decimal Height);
}
