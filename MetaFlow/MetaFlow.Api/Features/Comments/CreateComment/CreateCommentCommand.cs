using MetaFlow.Contracts.Comments;
using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Comments.CreateComment
{
    public record CreateCommentCommand(
        Guid CardId,
        string Content,
        Guid? ParentCommentId,
        Guid UserId
        ) : ICommand<CommentResponse>;
}
