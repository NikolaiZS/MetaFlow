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