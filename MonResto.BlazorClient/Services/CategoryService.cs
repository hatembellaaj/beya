using System.Net.Http.Json;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class CategoryService
{
    private readonly HttpClient _http;

    public CategoryService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<CategoryModel>> GetCategories()
    {
        return await _http.GetFromJsonAsync<IEnumerable<CategoryModel>>("api/categories") ?? Enumerable.Empty<CategoryModel>();
    }
}
