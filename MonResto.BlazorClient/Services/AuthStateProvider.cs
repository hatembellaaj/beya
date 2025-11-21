using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace MonResto.BlazorClient.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private string? _token;

    public string? Token => _token;

    public void SetUser(string userName, string token)
    {
        _token = token;
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "jwt");
        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void Logout()
    {
        _token = null;
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }
}
