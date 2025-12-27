using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Users
{
    public record AuthResponse(
    Guid UserId,
    string Email,
    string Username,
    string? FullName,
    string Token,
    DateTime ExpiresAt
);

}
