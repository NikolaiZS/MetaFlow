using MetaFlow.Api.Common;
using MetaFlow.Contracts.Attachments;

namespace MetaFlow.Api.Features.Attachments.GetAttachments
{
    public record GetAttachmentsQuery(
        Guid CardId,
        Guid UserId
        ) : IQuery<List<AttachmentResponse>>;
}
