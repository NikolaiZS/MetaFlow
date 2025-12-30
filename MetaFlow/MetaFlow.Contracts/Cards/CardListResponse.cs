using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Cards
{
    public record CardListResponse(
        Guid Id,
        string Title,
        string Priority,
        string Status,
        DateTime? DueDate,
        Guid? AssignedToId,
        string? AssignedToUsername,
        int CommentsCount,
        int AttachmentCount,
        DateTime UpdatedAt
        );
}
