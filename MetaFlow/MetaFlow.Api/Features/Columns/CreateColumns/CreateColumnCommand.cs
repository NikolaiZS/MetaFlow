using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Columns;

namespace MetaFlow.Api.Features.Columns.CreateColumns
{
    public record CreateColumnCommand(
    Guid BoardId,
    string Name,
    string? Description,
    string ColumnType,
    int? WipLimit,
    string Color,
    Guid UserId
) : ICommand<ColumnResponse>;
}