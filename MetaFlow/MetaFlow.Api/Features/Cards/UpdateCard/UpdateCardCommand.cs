using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;

namespace MetaFlow.Api.Features.Cards.UpdateCard
{
    public record UpdateCardCommand(
    Guid CardId,
    Guid UserId,
    string? Title,
    string? Description,
    string? Priority,
    string? Status,
    DateTime? DueDate,
    DateTime? StartDate,
    Guid? AssignedToId
) : ICommand<CardResponse>;
}
