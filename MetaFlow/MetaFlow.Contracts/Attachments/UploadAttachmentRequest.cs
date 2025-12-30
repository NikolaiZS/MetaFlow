namespace MetaFlow.Contracts.Attachments;

public record UploadAttachmentRequest(
    string FileName,
    string FileUrl,
    long FileSize,
    string MimeType,
    string? ThumbnailUrl = null
);
