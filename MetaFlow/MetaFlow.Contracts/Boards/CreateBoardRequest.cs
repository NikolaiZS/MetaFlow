using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Boards
{
    public record CreateBoardRequest(
    string Name,
    string? Description,
    Guid MethodologyPresetId,
    bool IsPublic = false,
    bool IsTemplate = false
);

}
