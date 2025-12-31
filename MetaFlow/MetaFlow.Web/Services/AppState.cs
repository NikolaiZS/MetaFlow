
using MetaFlow.Contracts.Users;

namespace MetaFlow.Web.Services;

public class AppState
{
    public AuthResponse? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public event Action? OnChange;

    public void SetUser(AuthResponse user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
