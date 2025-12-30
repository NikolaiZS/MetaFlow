using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;

namespace MetaFlow.Api.Features.Cards.CreateCard
{
    public record CreateCardCommand(
        Guid BoardId,
        string Title,
        string? Description,
        Guid ColumnId,
        string Priority,
        DateTime? DueDate,
        DateTime? StartDate,
        Guid? AssignedToId,
        Guid UserId
        ) : ICommand<CardResponse>;

}
