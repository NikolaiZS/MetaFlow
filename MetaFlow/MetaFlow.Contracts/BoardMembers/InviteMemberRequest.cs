using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.BoardMembers
{
    public record InviteMemberRequest(
    Guid UserId,
    string Role = "member"
    );
}
