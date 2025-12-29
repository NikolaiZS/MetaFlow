using MetaFlow.Api.Common;
using MetaFlow.Contracts.Columns;

namespace MetaFlow.Api.Features.Columns.GetColumns
{
    public record GetColumnsQuery(
    Guid BoardId,
    Guid UserId
) : IQuery<List<ColumnListResponse>>;
}
