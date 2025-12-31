namespace MetaFlow.Contracts.Columns
{
    public record UpdateColumnRequest(
    string? Name,
    string? Description,
    int? WipLimit,
    string? Color,
    bool? IsVisible
);
}