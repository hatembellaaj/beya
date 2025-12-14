using System.Net.Http.Json;
using System.Linq;
using MonResto.BlazorClient.Models;
using Microsoft.AspNetCore.Identity;

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

    public async Task<(bool Success, string? Error)> Register(string userName, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/account/register", new { userName, email, password });
        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }

        var errors = await response.Content.ReadFromJsonAsync<IEnumerable<IdentityError>>();
        var message = errors is null
            ? "Echec de l'inscription."
            : string.Join("; ", errors.Select(e => e.Description));
        return (false, message);
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
