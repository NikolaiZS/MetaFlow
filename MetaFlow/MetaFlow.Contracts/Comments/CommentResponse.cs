using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Comments
{
    public record CommentResponse(
        Guid Id,
        Guid CardId,
        Guid UserId,
        string Username,
        string? UserAvatarUrl,
        Guid? ParentCommentId,
        string Content,
        bool IsEdited,
        bool IsDeleted,
        DateTime CreatedAt,
        DateTime UpdateAt,
        List<CommentResponse> Replies
        );
}
