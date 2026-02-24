using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Columns;

namespace MetaFlow.Api.Features.Columns.UpdateColumn
{
    public record UpdateColumnCommand(
    Guid ColumnId,
    Guid UserId,
    string? Name,
    string? Description,
    int? WipLimit,
    string? Color,
    bool? IsVisible
) : ICommand<ColumnResponse>;
}