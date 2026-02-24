using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserResponse>>
{
    private readonly SupabaseService _supabaseService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cache;

    public GetCurrentUserHandler(
        SupabaseService supabaseService,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cache)
    {
        _supabaseService = supabaseService;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
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

        var cacheKey = $"user:{userId}";
        var cached = await _cache.GetAsync<UserResponse>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
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

        await _cache.SetAsync(cacheKey, userResponse, TimeSpan.FromMinutes(5), cancellationToken);

        return Result.Success(userResponse);
    }
}