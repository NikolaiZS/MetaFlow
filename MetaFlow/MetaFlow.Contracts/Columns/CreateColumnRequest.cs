using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Columns
{
    public record CreateColumnRequest(
    string Name,
    string? Description,
    string ColumnType = "standard",
    int? WipLimit = null,
    string Color = "#e0e0e0"
);
}
