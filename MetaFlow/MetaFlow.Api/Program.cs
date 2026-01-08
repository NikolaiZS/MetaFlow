using Carter;
using MetaFlow.Api.Common.Extensions;
using MetaFlow.Infrastructure.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddSwaggerGen(c =>
{
    Debug.WriteLine("!!!Swagger started!!!!");
    c.SwaggerDoc("v1", new() { Title = "MetaFlow API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSingleton<SupabaseService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new SupabaseService(
        config["Supabase:Url"]!,
        config["Supabase:ServiceRoleKey"]!
    );
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

try
{
    var supabase = app.Services.GetRequiredService<SupabaseService>();
    await supabase.InitializeAsync();
    app.Logger.LogInformation("✅ Supabase initialized successfully");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "❌ Supabase initialization failed");
    throw;
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

app.Run();