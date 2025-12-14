using System.Net.Http.Json;
using System.Net;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class CartService : ApiServiceBase
{
    public CartService(HttpClient http, AuthStateProvider authStateProvider) : base(http, authStateProvider)
    {
    }

    public async Task<(bool Success, IEnumerable<CartItemModel> Items, string? Error)> GetCart()
    {
        ApplyToken();
        if (!IsAuthenticated)
        {
            return (false, Enumerable.Empty<CartItemModel>(), "Connectez-vous pour accéder à votre panier.");
        }

        var response = await Http.GetAsync("api/cart");
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return (false, Enumerable.Empty<CartItemModel>(), "Connectez-vous pour accéder à votre panier.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return (false, Enumerable.Empty<CartItemModel>(), "Impossible de charger le panier.");
        }

        var items = await response.Content.ReadFromJsonAsync<IEnumerable<CartItemModel>>() ?? Enumerable.Empty<CartItemModel>();
        return (true, items, null);
    }

    public async Task<(bool Success, string? Error)> AddToCart(int articleId, int quantity)
    {
        ApplyToken();
        if (!IsAuthenticated)
        {
            return (false, "Vous devez être connecté pour ajouter un article au panier.");
        }

        var response = await Http.PostAsJsonAsync("api/cart", new { articleId, quantity });
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return (false, "Session expirée : reconnectez-vous avant d'ajouter au panier.");
        }

        return response.IsSuccessStatusCode
            ? (true, null)
            : (false, "Ajout au panier impossible pour le moment.");
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
