using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Boards
{
    public record UpdateBoardRequest(
    string? Name,
    string? Description,
    bool? IsPublic,
    bool? IsArchived
);

}
