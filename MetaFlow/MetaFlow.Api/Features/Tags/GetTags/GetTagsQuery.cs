using MetaFlow.Api.Common;
using MetaFlow.Contracts.Tags;

namespace MetaFlow.Api.Features.Tags.GetTags
{
    public record GetTagsQuery(
    Guid BoardId,
    Guid UserId
    ) : IQuery<List<TagResponse>>;
}
