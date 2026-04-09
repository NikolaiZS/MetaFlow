using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Comments.DeleteComment
{
    public record DeleteCommentCommand(
    Guid CommentId,
    Guid UserId
) : ICommand<bool>;
}
