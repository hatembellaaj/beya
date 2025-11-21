using System.Net.Http.Json;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class OrderService : ApiServiceBase
{
    public OrderService(HttpClient http, AuthStateProvider authStateProvider) : base(http, authStateProvider)
    {
    }

    public async Task<IEnumerable<OrderModel>> GetOrders()
    {
        ApplyToken();
        return await Http.GetFromJsonAsync<IEnumerable<OrderModel>>("api/order") ?? Enumerable.Empty<OrderModel>();
    }

    public async Task CreateOrder()
    {
        ApplyToken();
        await Http.PostAsync("api/order", null);
    }
}
