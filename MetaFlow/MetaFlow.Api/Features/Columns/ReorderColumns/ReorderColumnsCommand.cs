using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Columns;

namespace MetaFlow.Api.Features.Columns.ReorderColumns
{
    public record ReorderColumnsCommand(
    Guid BoardId,
    Guid UserId,
    List<ColumnPositionDto> Columns
) : ICommand<bool>;
}
