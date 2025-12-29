using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Columns
{
    public record UpdateColumnRequest(
    string? Name,
    string? Description,
    int? WipLimit,
    string? Color,
    bool? IsVisible
);
}
