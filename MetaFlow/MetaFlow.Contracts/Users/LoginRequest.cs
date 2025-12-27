using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Users
{
    public record LoginRequest(
    string EmailOrUsername,
    string Password
);

}
