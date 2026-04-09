using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly SupabaseService _supabaseService;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtService _jwtService;
    private readonly ICacheService _cache;

    public RegisterHandler(
        SupabaseService supabaseService,
        PasswordHasher passwordHasher,
        JwtService jwtService,
        ICacheService cache)
    {
        _supabaseService = supabaseService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _cache = cache;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var client = _supabaseService.GetClient();

        var emailLower = request.Email.ToLower();
        var usernameLower = request.Username.ToLower();

        var emailExistsKey = $"auth:email-exists:{emailLower}";
        var usernameExistsKey = $"auth:username-exists:{usernameLower}";

        var cachedEmailExists = await _cache.GetAsync<bool?>(emailExistsKey, cancellationToken);
        if (cachedEmailExists is true)
        {
            return Result.Failure<AuthResponse>("Email is already taken");
        }

        var cachedUsernameExists = await _cache.GetAsync<bool?>(usernameExistsKey, cancellationToken);
        if (cachedUsernameExists is true)
        {
            return Result.Failure<AuthResponse>("Username is already taken");
        }

        var emailCheck = await client
            .From<User>()
            .Select("id")
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, emailLower)
            .Get();

        if (emailCheck.Models.Count > 0)
        {
            await _cache.SetAsync(emailExistsKey, true, TimeSpan.FromMinutes(10), cancellationToken);
            return Result.Failure<AuthResponse>("Email is already taken");
        }

        var usernameCheck = await client
            .From<User>()
            .Select("id")
            .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, usernameLower)
            .Get();

        if (usernameCheck.Models.Count > 0)
        {
            await _cache.SetAsync(usernameExistsKey, true, TimeSpan.FromMinutes(10), cancellationToken);
            return Result.Failure<AuthResponse>("Username is already taken");
        }

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = emailLower,
            Username = usernameLower,
            FullName = request.FullName,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            EmailVerified = false,
            Preferences = new UserPreferences
            {
                Theme = "light",
                Language = "en",
                EmailNotifications = true,
                PushNotifications = false,
                ItemsPerPage = 20,
                DateFormat = "YYYY-MM-DD",
                TimeZone = "UTC"
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var insertResponse = await client
            .From<User>()
            .Insert(user);

        var createdUser = insertResponse.Models.FirstOrDefault();

        if (createdUser == null)
        {
            return Result.Failure<AuthResponse>("Failed to create user");
        }

        // Cache uniqueness flags and user lookup by identifier for faster auth flows
        await _cache.SetAsync(emailExistsKey, true, TimeSpan.FromMinutes(10), cancellationToken);
        await _cache.SetAsync(usernameExistsKey, true, TimeSpan.FromMinutes(10), cancellationToken);

        var cachedUser = new CachedAuthUser(
            createdUser.Id,
            createdUser.Email,
            createdUser.Username,
            createdUser.FullName,
            createdUser.AvatarUrl,
            createdUser.PasswordHash,
            createdUser.EmailVerified,
            createdUser.LastLoginAt,
            createdUser.CreatedAt,
            createdUser.UpdatedAt);

        await _cache.SetAsync($"auth:user:{emailLower}", cachedUser, TimeSpan.FromMinutes(10), cancellationToken);
        await _cache.SetAsync($"auth:user:{usernameLower}", cachedUser, TimeSpan.FromMinutes(10), cancellationToken);

        var token = _jwtService.GenerateToken(createdUser.Id, createdUser.Email, createdUser.Username);
        var expiresAt = DateTime.UtcNow.AddMinutes(1440);

        var response = new AuthResponse(
            createdUser.Id,
            createdUser.Email,
            createdUser.Username,
            createdUser.FullName,
            token,
            expiresAt
        );

        return Result.Success(response);
    }
}