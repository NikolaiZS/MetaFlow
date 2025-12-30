using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Cards
{
    public record MoveCardRequest(
        Guid TargetColumnId,
        double Position
        );
}
