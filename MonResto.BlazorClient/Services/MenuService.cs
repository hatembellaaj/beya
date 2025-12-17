using System.Net.Http.Json;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class MenuService
{
    private readonly HttpClient _http;

    public MenuService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<MenuModel>> GetMenus()
    {
        return await _http.GetFromJsonAsync<IEnumerable<MenuModel>>("api/menu")
               ?? Enumerable.Empty<MenuModel>();
    }

    public async Task<MenuModel?> GetMenu(int id)
    {
        return await _http.GetFromJsonAsync<MenuModel>($"api/menu/{id}");
    }
}
