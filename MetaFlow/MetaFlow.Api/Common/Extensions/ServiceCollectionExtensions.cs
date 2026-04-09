using Carter;
using FluentValidation;
using MetaFlow.Api.Common.Behaviors;
using MetaFlow.Api.Common.Exceptions;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;
using MetaFlow.Infrastructure.Services.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace MetaFlow.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCarter();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddScoped<PasswordHasher>();
        services.AddScoped<JwtService>();
        services.AddHttpContextAccessor();

        services.AddStackExchangeRedisCache(options =>
        {
            var redisConnection = configuration.GetConnectionString("Redis")
                ?? configuration["Redis:ConnectionString"]
                ?? "localhost:6379";
            
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
            {
                EndPoints = { ExtractRedisEndpoint(redisConnection) },
                Password = ExtractRedisPassword(redisConnection),
                Ssl = redisConnection.StartsWith("rediss://"),
                AbortOnConnectFail = false,
                ConnectTimeout = 10000,
                SyncTimeout = 10000
            };
            
            options.InstanceName = configuration["Redis:InstanceName"] ?? "metaflow:";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        // Email Service Client
        var emailServiceUrl = configuration["EmailService:BaseUrl"] ?? "http://localhost:8081";
        services.AddHttpClient<IEmailServiceClient, EmailServiceClient>(client =>
        {
            client.BaseAddress = new Uri(emailServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        var jwtSecret = configuration["JwtSettings:Secret"]!;
        var jwtIssuer = configuration["JwtSettings:Issuer"]!;
        var jwtAudience = configuration["JwtSettings:Audience"]!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static string ExtractRedisEndpoint(string connectionString)
    {
        // Поддержка форматов: rediss://user:pass@host:port или host:port
        if (connectionString.Contains("://"))
        {
            var uri = new Uri(connectionString.Replace("rediss://", "https://").Replace("redis://", "http://"));
            return $"{uri.Host}:{uri.Port}";
        }
        return connectionString;
    }

    private static string? ExtractRedisPassword(string connectionString)
    {
        // Извлекаем пароль из rediss://default:password@host:port
        if (connectionString.Contains("://"))
        {
            var uri = new Uri(connectionString.Replace("rediss://", "https://").Replace("redis://", "http://"));
            var userInfo = uri.UserInfo;
            if (userInfo.Contains(":"))
            {
                return userInfo.Split(':', 2)[1];
            }
        }
        return null;
    }
}