using MetaFlow.Api.Common;
using MetaFlow.Contracts.Cards;

namespace MetaFlow.Api.Features.Cards.GetCard
{
    public record GetCardQuery(
    Guid CardId,
    Guid UserId
) : IQuery<CardResponse>;
}