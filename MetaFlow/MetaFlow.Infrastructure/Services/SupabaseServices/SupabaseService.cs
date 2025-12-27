using Supabase;

namespace MetaFlow.Infrastructure.Services;

public class SupabaseService
{
    private readonly string _url;
    private readonly string _key;
    private Client? _client;

    public SupabaseService(string url, string key)
    {
        _url = url;
        _key = key;
    }

    public async Task InitializeAsync()
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = true
        };

        _client = new Client(_url, _key, options);
        await _client.InitializeAsync();
    }

    public Client GetClient()
    {
        if (_client == null)
        {
            throw new InvalidOperationException("Supabase client is not initialized. Call InitializeAsync first.");
        }

        return _client;
    }
}
