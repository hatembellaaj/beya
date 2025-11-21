using System.Net.Http.Json;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class CartService : ApiServiceBase
{
    public CartService(HttpClient http, AuthStateProvider authStateProvider) : base(http, authStateProvider)
    {
    }

    public async Task<IEnumerable<CartItemModel>> GetCart()
    {
        ApplyToken();
        return await Http.GetFromJsonAsync<IEnumerable<CartItemModel>>("api/cart") ?? Enumerable.Empty<CartItemModel>();
    }

    public async Task AddToCart(int articleId, int quantity)
    {
        ApplyToken();
        await Http.PostAsJsonAsync("api/cart", new { articleId, quantity });
    }

    public async Task UpdateQuantity(int cartItemId, int quantity)
    {
        ApplyToken();
        await Http.PutAsJsonAsync($"api/cart/{cartItemId}", new { articleId = 0, quantity });
    }

    public async Task Remove(int cartItemId)
    {
        ApplyToken();
        await Http.DeleteAsync($"api/cart/{cartItemId}");
    }
}
