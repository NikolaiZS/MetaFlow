using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Cards
{
    public record UpdateCardRequest(
        string? Title,
        string? Description,
        string? Priority,
        string? Status,
        DateTime? DueDate,
        DateTime? StartTime,
        Guid? AssignedToId
        );
}
