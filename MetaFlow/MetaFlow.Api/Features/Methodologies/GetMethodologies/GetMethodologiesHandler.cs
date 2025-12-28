using MediatR;
using MetaFlow.Api.Common;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Methodologies;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Methodologies.GetMethodologies
{
    public class GetMethodologiesHandler : IRequestHandler<GetMethodologiesQuery, Result<List<MethodologyResponse>>>
    {
        private readonly SupabaseService _supabaseService;

        public GetMethodologiesHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<List<MethodologyResponse>>> Handle(
            GetMethodologiesQuery request,
            CancellationToken cancellationToken)
        {
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

            return Result.Success(methodologies);
        }
    }


}
