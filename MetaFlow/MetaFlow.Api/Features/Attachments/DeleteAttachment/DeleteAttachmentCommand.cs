using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Attachments.DeleteAttachment
{
    public record DeleteAttachmentCommand(
    Guid AttachmentId,
    Guid UserId
) : ICommand<bool>;
}
