using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Methodologies;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Methodologies.GetMethodologies
{
    public class GetMethodologiesHandler : IRequestHandler<GetMethodologiesQuery, Result<List<MethodologyResponse>>>
    {
        private readonly SupabaseService _supabaseService;
        private readonly ICacheService _cache;

        public GetMethodologiesHandler(SupabaseService supabaseService, ICacheService cache)
        {
            _supabaseService = supabaseService;
            _cache = cache;
        }

        public async Task<Result<List<MethodologyResponse>>> Handle(
            GetMethodologiesQuery request,
            CancellationToken cancellationToken)
        {
            var cacheKey = "methodologies:active";
            var cached = await _cache.GetAsync<List<MethodologyResponse>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return Result.Success(cached);
            }

            var client = _supabaseService.GetClient();

            var response = await client
                .From<MethodologyPreset>()
                .Select("id,name,display_name,description,icon,category,is_system,is_active,created_at,updated_at")
                .Filter("is_active", Supabase.Postgrest.Constants.Operator.Equals, "true")
                .Order("display_name", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            var methodologies = response.Models.Select(m => new MethodologyResponse(
                m.Id,
                m.Name,
                m.DisplayName,
                m.Description,
                m.Icon,
                m.Category,
                m.IsSystem,
                m.IsActive
            )).ToList();

            await _cache.SetAsync(cacheKey, methodologies, TimeSpan.FromMinutes(60), cancellationToken);

            return Result.Success(methodologies);
        }
    }
}