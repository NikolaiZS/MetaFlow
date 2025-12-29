using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Columns
{
    public record ColumnResponse(
    Guid Id,
    Guid BoardId,
    string Name,
    string? Description,
    int Position,
    string ColumnType,
    int? WipLimit,
    string Color,
    bool IsVisible,
    int CardCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
}
