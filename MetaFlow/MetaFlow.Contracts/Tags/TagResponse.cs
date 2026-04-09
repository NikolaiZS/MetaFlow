using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Tags
{
    public record TagResponse(
    Guid Id,
    Guid BoardId,
    string Name,
    string Color,
    DateTime CreatedAt
);
}
