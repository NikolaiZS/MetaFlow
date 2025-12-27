using MediatR;
using MetaFlow.Api.Common;
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
        var emailOrUsername = request.EmailOrUsername.ToLower();

        // Получаем все пользователи (в реальности будет 1 или 0)
        var response = await client
            .From<User>()
            .Get();

        // Фильтруем в памяти
        var user = response.Models
            .FirstOrDefault(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

        if (user == null)
        {
            return Result.Failure<AuthResponse>("Invalid credentials");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash!))
        {
            return Result.Failure<AuthResponse>("Invalid credentials");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await client
            .From<User>()
            .Update(user);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Username);
        var expiresAt = DateTime.UtcNow.AddMinutes(1440);

        var authResponse = new AuthResponse(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            token,
            expiresAt
        );

        return Result.Success(authResponse);
    }

}
