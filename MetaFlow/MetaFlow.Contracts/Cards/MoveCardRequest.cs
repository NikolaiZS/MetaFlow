namespace MetaFlow.Contracts.Cards
{
    public record MoveCardRequest(
        Guid TargetColumnId,
        double Position
        );
}