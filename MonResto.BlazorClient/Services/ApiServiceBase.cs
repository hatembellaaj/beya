using System.Net.Http.Headers;

namespace MonResto.BlazorClient.Services;

public abstract class ApiServiceBase
{
    protected readonly HttpClient Http;
    private readonly AuthStateProvider _authStateProvider;

    protected bool IsAuthenticated => !string.IsNullOrWhiteSpace(_authStateProvider.Token);

    protected ApiServiceBase(HttpClient http, AuthStateProvider authStateProvider)
    {
        Http = http;
        _authStateProvider = authStateProvider;
    }

    protected void ApplyToken()
    {
        if (!string.IsNullOrWhiteSpace(_authStateProvider.Token))
        {
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authStateProvider.Token);
        }
        else
        {
            Http.DefaultRequestHeaders.Authorization = null;
        }
    }
}
