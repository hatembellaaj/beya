using System.Net.Http.Json;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class ArticleService
{
    private readonly HttpClient _http;

    public ArticleService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ArticleModel>> GetArticles()
    {
        return await _http.GetFromJsonAsync<IEnumerable<ArticleModel>>("api/articles") ?? Enumerable.Empty<ArticleModel>();
    }

    public async Task<ArticleModel?> GetArticle(int id) => await _http.GetFromJsonAsync<ArticleModel>($"api/articles/{id}");
    public async Task<IEnumerable<ArticleModel>> GetByCategory(int categoryId) => await _http.GetFromJsonAsync<IEnumerable<ArticleModel>>($"api/articles/by-category/{categoryId}") ?? Enumerable.Empty<ArticleModel>();
}
