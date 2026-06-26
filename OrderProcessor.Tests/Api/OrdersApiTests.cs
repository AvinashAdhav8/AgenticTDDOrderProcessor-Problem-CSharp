using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderProcessor.Storage;

namespace OrderProcessor.Tests.Api;

public class InMemoryAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder) =>
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IOrderStore>();
            services.AddSingleton<IOrderStore, InMemoryOrderStore>();
        });
}

public class OrdersApiTests : IClassFixture<InMemoryAppFactory>
{
    private readonly InMemoryAppFactory _factory;

    public OrdersApiTests(InMemoryAppFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrderWithZeroTotal()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/api/orders", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);
        Assert.NotEqual(Guid.Empty, order!.Id);
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public async Task ListOrders_ReturnsCreatedOrders()
    {
        var client = _factory.CreateClient();
        var created = await client.PostAsync("/api/orders", null);
        var createdOrder = await created.Content.ReadFromJsonAsync<OrderResponse>();

        var response = await client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
        Assert.Contains(orders!, o => o.Id == createdOrder!.Id);
    }

    [Fact]
    public async Task AddProduct_UpdatesOrderTotal()
    {
        var client = _factory.CreateClient();
        var created = await client.PostAsync("/api/orders", null);
        var order = await created.Content.ReadFromJsonAsync<OrderResponse>();

        var response = await client.PostAsJsonAsync(
            $"/api/orders/{order!.Id}/products",
            new { price = 10.00m, discount = 0.10m });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.Equal(9.00m, updated!.Total);
    }

    [Fact]
    public async Task AddProduct_PersistsAllProductFields()
    {
        var client = _factory.CreateClient();
        var created = await client.PostAsync("/api/orders", null);
        var order = await created.Content.ReadFromJsonAsync<OrderResponse>();

        var product = new
        {
            name = "Wine Glass",
            color = "Red",
            size = "Small",
            price = 12.50m,
            discount = 0.20m,
            material = "Glass",
            weight = 0.3m,
            fragility = true,
            containsLiquids = false,
            packaging = "Boxed",
            dimensions = new { length = 8.0m, width = 8.0m, height = 20.0m },
        };
        await client.PostAsJsonAsync($"/api/orders/{order!.Id}/products", product);

        var orders = await client.GetFromJsonAsync<List<OrderDetail>>("/api/orders");
        var saved = orders!.Single(o => o.Id == order.Id).Products.Single();

        Assert.Equal("Wine Glass", saved.Name);
        Assert.Equal("Red", saved.Color);
        Assert.Equal("Small", saved.Size);
        Assert.Equal(12.50m, saved.Price);
        Assert.Equal(0.20m, saved.Discount);
        Assert.Equal("Glass", saved.Material);
        Assert.Equal(0.3m, saved.Weight);
        Assert.True(saved.Fragility);
        Assert.False(saved.ContainsLiquids);
        Assert.Equal("Boxed", saved.Packaging);
        Assert.Equal(8.0m, saved.Dimensions.Length);
        Assert.Equal(8.0m, saved.Dimensions.Width);
        Assert.Equal(20.0m, saved.Dimensions.Height);
    }

    private record OrderResponse(Guid Id, decimal Total);

    private record OrderDetail(Guid Id, decimal Total, List<ProductDetail> Products);

    private record ProductDetail(
        string Name, string Color, string Size, decimal Price, decimal Discount,
        string Material, decimal Weight, bool Fragility, bool ContainsLiquids,
        string Packaging, DimensionsDetail Dimensions);

    private record DimensionsDetail(decimal Length, decimal Width, decimal Height);
}