using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.SupabaseServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SupabaseService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var url = config["Supabase:Url"]!;
    var key = config["Supabase:ServiceRoleKey"]!;
    return new SupabaseService(url, key);
});

var app = builder.Build();

var supabaseService = app.Services.GetRequiredService<SupabaseService>();
await supabaseService.InitializeAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "MetaFlow API is running!");

app.MapGet("/test-supabase", async (IConfiguration config) =>
{
    try
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("apikey", config["Supabase:ServiceRoleKey"]!);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["Supabase:ServiceRoleKey"]}");

        var url = $"{config["Supabase:Url"]}/rest/v1/";
        var response = await httpClient.GetAsync(url);

        return Results.Ok(new
        {
            status = "connected",
            clientInitialized = true,
            apiAccessible = response.IsSuccessStatusCode,
            statusCode = (int)response.StatusCode
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});



app.Run();
