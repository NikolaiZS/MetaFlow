using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly SupabaseService _supabaseService;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtService _jwtService;

    public LoginHandler(
        SupabaseService supabaseService,
        PasswordHasher passwordHasher,
        JwtService jwtService)
    {
        _supabaseService = supabaseService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var client = _supabaseService.GetClient();
        var identifier = request.EmailOrUsername.ToLower();

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