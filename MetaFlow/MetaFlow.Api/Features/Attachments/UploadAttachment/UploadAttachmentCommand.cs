using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Attachments;

namespace MetaFlow.Api.Features.Attachments.UploadAttachment
{
    public record UploadAttachmentCommand(
    Guid CardId,
    string FileName,
    string FileUrl,
    long FileSize,
    string MimeType,
    string? ThumbnailUrl,
    Guid UserId
    ) : ICommand<AttachmentResponse>;
}
