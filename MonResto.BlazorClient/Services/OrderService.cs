using System.Net.Http.Json;
using MonResto.BlazorClient.Models;
using System.Net;

namespace MonResto.BlazorClient.Services;

public class OrderService : ApiServiceBase
{
    public OrderService(HttpClient http, AuthStateProvider authStateProvider) : base(http, authStateProvider)
    {
    }

    public async Task<IEnumerable<OrderModel>> GetOrders()
    {
        ApplyToken();
        var response = await Http.GetAsync("api/order");
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Enumerable.Empty<OrderModel>();
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<OrderModel>>() ?? Enumerable.Empty<OrderModel>();
    }

    public async Task CreateOrder()
    {
        ApplyToken();
        await Http.PostAsync("api/order", null);
    }
}
