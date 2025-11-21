using System.Net.Http.Json;
using MonResto.BlazorClient.Models;

namespace MonResto.BlazorClient.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly AuthStateProvider _authStateProvider;

    public AuthService(HttpClient http, AuthStateProvider authStateProvider)
    {
        _http = http;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> Register(string userName, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/account/register", new { userName, email, password });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Login(string userName, string password)
    {
        var response = await _http.PostAsJsonAsync("api/account/login", new { userName, password });
        if (!response.IsSuccessStatusCode) return false;
        var token = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (token is null) return false;
        _authStateProvider.SetUser(token.UserName, token.Token);
        return true;
    }

    public void Logout() => _authStateProvider.Logout();
}
