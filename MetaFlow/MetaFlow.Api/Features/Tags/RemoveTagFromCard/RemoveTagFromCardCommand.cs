using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Tags.RemoveTagFromCard
{
    public record RemoveTagFromCardCommand(
    Guid CardId,
    Guid TagId,
    Guid UserId
    ) : ICommand<bool>;
}
