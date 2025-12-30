using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Cards
{
    public record CardResponse(
    Guid Id,
    Guid BoardId,
    Guid ColumnId,
    string ColumnName,
    string Title,
    string? Description,
    double Position,
    string Priority,
    string Status,
    DateTime? DueDate,
    DateTime? StartDate,
    DateTime? CompletedAt,
    Guid CreatedById,
    string CreatedByUsername,
    Guid? AssignedToId,
    string? AssignedToUsername,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
}
