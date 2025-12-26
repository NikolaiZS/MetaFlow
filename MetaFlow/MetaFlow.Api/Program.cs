using Carter;
using MetaFlow.Api.Common.Extensions;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.SupabaseServices;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);

// Supabase
builder.Services.AddSingleton<SupabaseService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new SupabaseService(
        config["Supabase:Url"]!,
        config["Supabase:ServiceRoleKey"]!
    );
});

var app = builder.Build();

// Initialize Supabase
try
{
    var supabase = app.Services.GetRequiredService<SupabaseService>();
    await supabase.InitializeAsync();
    app.Logger.LogInformation("? Supabase initialized successfully");
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "?? Failed to initialize Supabase");
}

// Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map Carter endpoints
app.MapCarter();

app.Run();
