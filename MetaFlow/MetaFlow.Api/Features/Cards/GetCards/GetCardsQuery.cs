using MetaFlow.Api.Common;
using MetaFlow.Contracts.Cards;

namespace MetaFlow.Api.Features.Cards.GetCards
{
    public record GetCardsQuery(
    Guid BoardId,
    Guid? ColumnId,
    Guid UserId,
    bool IncludeArchived = false
) : IQuery<List<CardListResponse>>;
}