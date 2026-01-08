using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;

namespace MetaFlow.Api.Features.Tags.CreateTag
{
    public record CreateTagCommand(
    Guid BoardId,
    string Name,
    string Color,
    Guid UserId
) : ICommand<TagResponse>;
}
