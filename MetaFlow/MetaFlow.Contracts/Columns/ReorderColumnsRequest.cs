using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Columns
{
    public record ReorderColumnsRequest(
    List<ColumnPositionDto> Columns
);

    public record ColumnPositionDto(
        Guid ColumnId,
        int Position
    );
}
