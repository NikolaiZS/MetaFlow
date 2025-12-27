using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Methodologies
{
    public record MethodologyResponse(
    Guid Id,
    string Name,
    string DisplayName,
    string? Description,
    string? Icon,
    string? Category,
    bool IsSystem,
    bool IsActive
);

}
