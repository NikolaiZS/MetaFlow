using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.BoardMembers
{
    public record BoardMemberResponse(
    Guid Id,
    Guid BoardId,
    Guid UserId,
    string Username,
    string? FullName,
    string? AvatarUrl,
    string Role,
    DateTime JoinedAt,
    Guid? InvitedBy,
    string? InvitedByUsername
    );
}
