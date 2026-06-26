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
            WithOrder(id, store,
                order => order.AddProduct(request.ToProduct()),
                order => Results.Created($"/api/orders/{order.Id}", ToResponse(order))));

        app.MapPost("/api/orders/{id:guid}/confirm", (Guid id, IOrderStore store) =>
        {
            var order = store.Find(id);
            if (order is null) return Results.NotFound();
            try
            {
                order.Confirm();
            }
            catch (InvalidOperationException)
            {
                return Results.Conflict();
            }
            store.Update(order);
            return Results.Ok(ToResponse(order));
        });
    }

    private static IResult WithOrder(Guid id, IOrderStore store, Action<Order> mutate, Func<Order, IResult> respond)
    {
        var order = store.Find(id);
        if (order is null) return Results.NotFound();
        mutate(order);
        store.Update(order);
        return respond(order);
    }

    private static object ToResponse(Order o) => new
    {
        o.Id,
        o.Total,
        Status = o.Status,
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
            Dimensions = Dimensions?.ToDimensions() ?? default,
        };
    }

    private record DimensionsRequest(decimal Length, decimal Width, decimal Height)
    {
        public Dimensions ToDimensions() => new(Length, Width, Height);
    }
}
