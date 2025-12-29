using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Columns
{
    public record ColumnListResponse(
    Guid Id,
    string Name,
    int Position,
    string ColumnType,
    int? WipLimit,
    string Color,
    bool IsVisible,
    int CardCount
);
}
