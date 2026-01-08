using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Tags.AddTagToCard
{
    public record AddTagToCardCommand(
    Guid CardId,
    Guid TagId,
    Guid UserId
    ) : ICommand<bool>;
}
