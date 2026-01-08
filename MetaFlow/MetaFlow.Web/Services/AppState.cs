
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MetaFlow.Contracts.Users;

namespace MetaFlow.Web.Services;

public class AppState
{
    private readonly ProtectedSessionStorage _sessionStorage;

    public AppState(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public AuthResponse? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<AuthResponse>("user");
            if (result.Success)
            {
                CurrentUser = result.Value;
                NotifyStateChanged();
            }
        }
        catch
        {
            
        }
    }

    public async Task SetUser(AuthResponse user)
    {
        CurrentUser = user;
        await _sessionStorage.SetAsync("user", user);
        NotifyStateChanged();
    }

    public async Task Logout()
    {
        CurrentUser = null;
        await _sessionStorage.DeleteAsync("user");
        NotifyStateChanged();
    }

    public List<ApiResponseInfo> ApiRequestLog { get; private set; } = new();

    public event Action? OnLogChange;

    public void UpdateLastResponse(string method, string url, int statusCode, string? body = null, Dictionary<string, IEnumerable<string>>? requestHeaders = null, Dictionary<string, IEnumerable<string>>? responseHeaders = null)
    {
        ApiRequestLog.Insert(0, new ApiResponseInfo(method, url, statusCode, body, requestHeaders, responseHeaders, DateTime.Now));
        if (ApiRequestLog.Count > 50)
        {
            ApiRequestLog.RemoveRange(50, ApiRequestLog.Count - 50);
        }
        OnLogChange?.Invoke();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

public record ApiResponseInfo(string Method, string Url, int StatusCode, string? Body, Dictionary<string, IEnumerable<string>>? RequestHeaders, Dictionary<string, IEnumerable<string>>? ResponseHeaders, DateTime Timestamp);
