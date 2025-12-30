using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Comments;

namespace MetaFlow.Api.Features.Comments.CreateComment
{
    public record CreateCommentCommand(
        Guid CardId,
        string Content,
        Guid? ParentCommentId,
        Guid UserId
        ) : ICommand<CommentResponse>;
}