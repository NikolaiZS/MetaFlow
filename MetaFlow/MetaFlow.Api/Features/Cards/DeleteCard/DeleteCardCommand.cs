using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Cards.DeleteCard
{
    public record DeleteCardCommand(
    Guid CardId,
    Guid UserId
) : ICommand<bool>;
}