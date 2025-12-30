using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Cards.ArchiveCard
{
    public record ArchiveCardCommand(
    Guid CardId,
    Guid UserId,
    bool Archive = true
) : ICommand<bool>;
}
