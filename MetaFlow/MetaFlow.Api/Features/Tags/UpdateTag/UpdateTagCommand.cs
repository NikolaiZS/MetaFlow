using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;

namespace MetaFlow.Api.Features.Tags.UpdateTag
{
    public record UpdateTagCommand(
    Guid TagId,
    Guid UserId,
    string? Name,
    string? Color
    ) : ICommand<TagResponse>;
}
