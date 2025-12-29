using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserResponse>>
{
    private readonly SupabaseService _supabaseService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentUserHandler(
        SupabaseService supabaseService,
        IHttpContextAccessor httpContextAccessor)
    {
        _supabaseService = supabaseService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<UserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Result.Failure<UserResponse>("User not authenticated");
        }

        var client = _supabaseService.GetClient();

        var response = await client
            .From<User>()
            .Where(u => u.Id == userId)
            .Single();

        if (response == null)
        {
            return Result.Failure<UserResponse>("User not found");
        }

        var userResponse = new UserResponse(
            response.Id,
            response.Email,
            response.Username,
            response.FullName,
            response.AvatarUrl,
            response.EmailVerified,
            response.CreatedAt
        );

        return Result.Success(userResponse);
    }
}
