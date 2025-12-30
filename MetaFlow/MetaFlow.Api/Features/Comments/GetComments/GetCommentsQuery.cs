using MetaFlow.Contracts.Comments;
using MetaFlow.Api.Common;

namespace MetaFlow.Api.Features.Comments.GetComments
{
    public record GetCommentsQuery(
        Guid CardId,
        Guid UserId
        ) : IQuery<List<CommentResponse>>;

}
