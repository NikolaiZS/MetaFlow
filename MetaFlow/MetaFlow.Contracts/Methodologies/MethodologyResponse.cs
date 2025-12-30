namespace MetaFlow.Contracts.Methodologies
{
    public record MethodologyResponse(
    Guid Id,
    string Name,
    string DisplayName,
    string? Description,
    string? Icon,
    string? Category,
    bool IsSystem,
    bool IsActive
);
}