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

    public async Task<string> UploadFileToBucketAsync(
        string bucketName,
        string objectPath,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var client = GetClient();

        var extension = Path.GetExtension(objectPath);
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");

        await using (var fileStream = File.Create(tempFile))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        var options = new Supabase.Storage.FileOptions
        {
            CacheControl = "3600",
            Upsert = false
        };

        await client.Storage
            .From(bucketName)
            .Upload(tempFile, objectPath, options);

        File.Delete(tempFile);

        var publicUrl = client.Storage
            .From(bucketName)
            .GetPublicUrl(objectPath);

        return publicUrl;
    }
}