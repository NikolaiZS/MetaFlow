namespace MetaFlow.Contracts.Columns
{
    public record ColumnListResponse(
    Guid Id,
    string Name,
    int Position,
    string ColumnType,
    int? WipLimit,
    string Color,
    bool IsVisible,
    int CardCount
);
}