using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Comments;

namespace MetaFlow.Api.Features.Comments.UpdateComment
{
    public record UpdateCommentCommand(
    Guid CommentId,
    string Content,
    Guid UserId
) : ICommand<CommentResponse>;
}
