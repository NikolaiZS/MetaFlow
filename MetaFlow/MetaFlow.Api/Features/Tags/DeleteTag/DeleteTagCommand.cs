using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Tags.DeleteTag
{
    public record DeleteTagCommand(
    Guid TagId,
    Guid UserId
    ) : ICommand<bool>;
}
