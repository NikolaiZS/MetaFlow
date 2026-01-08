using MetaFlow.Api.Common;
using MetaFlow.Contracts.Tags;

namespace MetaFlow.Api.Features.Tags.GetCardTags
{
    public record GetCardTagsQuery(
    Guid CardId,
    Guid UserId
    ) : IQuery<List<TagResponse>>;
}
