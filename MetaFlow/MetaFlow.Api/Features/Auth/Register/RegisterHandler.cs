using MediatR;
using MetaFlow.Api.Common;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly SupabaseService _supabaseService;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtService _jwtService;

    public RegisterHandler(
        SupabaseService supabaseService,
        PasswordHasher passwordHasher,
        JwtService jwtService)
    {
        _supabaseService = supabaseService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var client = _supabaseService.GetClient();

        var emailLower = request.Email.ToLower();
        var usernameLower = request.Username.ToLower();

        // Check if email already exists
        var emailCheck = await client
            .From<User>()
            .Select("id")
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, emailLower)
            .Get();

        if (emailCheck.Models.Count > 0)
        {
            return Result.Failure<AuthResponse>("Email is already taken");
        }

        // Check if username already exists
        var usernameCheck = await client
            .From<User>()
            .Select("id")
            .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, usernameLower)
            .Get();

        if (usernameCheck.Models.Count > 0)
        {
            return Result.Failure<AuthResponse>("Username is already taken");
        }

        // Create user
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

        // Generate JWT token
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
