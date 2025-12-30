using MetaFlow.Api.Common;
using MetaFlow.Contracts.Methodologies;

namespace MetaFlow.Api.Features.Methodologies.GetMethodologies
{
    public record GetMethodologiesQuery : IQuery<List<MethodologyResponse>>;
}