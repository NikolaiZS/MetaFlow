using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly SupabaseService _supabaseService;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtService _jwtService;
    private readonly ICacheService _cache;

    public LoginHandler(
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
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var client = _supabaseService.GetClient();
        var identifier = request.EmailOrUsername.ToLower();

        var cacheKey = $"auth:user:{identifier}";
        var cachedUser = await _cache.GetAsync<CachedAuthUser>(cacheKey, cancellationToken);

        if (cachedUser is not null)
        {
            if (string.IsNullOrEmpty(cachedUser.PasswordHash) ||
                !_passwordHasher.VerifyPassword(request.Password, cachedUser.PasswordHash))
            {
                return Result.Failure<AuthResponse>("Invalid credentials");
            }

            var tokenFromCache = _jwtService.GenerateToken(cachedUser.Id, cachedUser.Email, cachedUser.Username);
            var expiresAtFromCache = DateTime.UtcNow.AddMinutes(1440);

            var responseFromCache = new AuthResponse(
                cachedUser.Id,
                cachedUser.Email,
                cachedUser.Username,
                cachedUser.FullName,
                tokenFromCache,
                expiresAtFromCache
            );

            return Result.Success(responseFromCache);
        }

        var userResponse = await client
            .From<User>()
            .Select("id,email,username,full_name,avatar_url,password_hash,email_verified,last_login_at,created_at,updated_at")
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, identifier)
            .Get();

        var user = userResponse.Models.FirstOrDefault();

        if (user == null)
        {
            userResponse = await client
                .From<User>()
                .Select("id,email,username,full_name,avatar_url,password_hash,email_verified,last_login_at,created_at,updated_at")
                .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, identifier)
                .Get();

            user = userResponse.Models.FirstOrDefault();
        }

        if (user == null)
        {
            return Result.Failure<AuthResponse>("Invalid credentials");
        }

        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponse>("Invalid credentials");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await client
            .From<User>()
            .Update(user);

        var cachedToStore = new CachedAuthUser(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            user.AvatarUrl,
            user.PasswordHash,
            user.EmailVerified,
            user.LastLoginAt,
            user.CreatedAt,
            user.UpdatedAt);

        await _cache.SetAsync(cacheKey, cachedToStore, TimeSpan.FromMinutes(60), cancellationToken);

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Username);
        var expiresAt = DateTime.UtcNow.AddMinutes(1440);

        var response = new AuthResponse(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            token,
            expiresAt
        );

        return Result.Success(response);
    }
}