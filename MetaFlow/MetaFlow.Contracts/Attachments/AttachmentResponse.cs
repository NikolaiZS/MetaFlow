using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Attachments
{
    public record AttachmentResponse(
    Guid Id,
    Guid CardId,
    Guid UploadedById,
    string UploadedByUsername,
    string FileName,
    string FileUrl,
    long? FileSize,
    string? MimeType,
    string? ThumbnailUrl,
    DateTime UploadedAt
    );
}
