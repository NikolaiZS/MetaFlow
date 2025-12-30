using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;

namespace MetaFlow.Api.Features.Cards.MoveCard
{
    public record MoveCardCommand(
    Guid CardId,
    Guid TargetColumnId,
    double Position,
    Guid UserId
) : ICommand<CardResponse>;
}