using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Columns.DeleteColumn
{
    public record DeleteColumnCommand(
    Guid ColumnId,
    Guid UserId
) : ICommand<bool>;
}