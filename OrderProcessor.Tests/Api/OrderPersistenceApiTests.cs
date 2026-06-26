using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OrderProcessor.Tests.Api;

public class OrderPersistenceApiTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"orders-api-{Guid.NewGuid():N}.json");

    private WebApplicationFactory<Program> NewApp() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("OrdersFilePath", _path));

    [Fact]
    public async Task OrdersWithProductsSurviveRestart()
    {
        Guid orderId;
        await using (var app = NewApp())
        {
            var client = app.CreateClient();
            var created = await client.PostAsync("/api/orders", null);
            var order = await created.Content.ReadFromJsonAsync<OrderResponse>();
            orderId = order!.Id;
            await client.PostAsJsonAsync(
                $"/api/orders/{orderId}/products",
                new { price = 10.00m, discount = 0.10m });
        }

        await using var restarted = NewApp();
        var orders = await restarted.CreateClient().GetFromJsonAsync<List<OrderResponse>>("/api/orders");

        var reloaded = orders!.Single(o => o.Id == orderId);
        Assert.Equal(9.00m, reloaded.Total);
    }

    private record OrderResponse(Guid Id, decimal Total);

    public void Dispose()
    {
        if (File.Exists(_path))
            File.Delete(_path);
    }
}
