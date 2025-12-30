using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Cards
{
    public record CreateCardRequest(
        string Title,
        string? Description,
        Guid ColumnId,
        string Priority = "medium",
        DateTime? DueDate = null,
        DateTime? StartDate = null,
        Guid? AssignedToId = null
        );
}
