using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("card_attachments")]
public class CardAttachment : BaseEntity
{
    [Column("card_id")]
    public Guid CardId { get; set; }

    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Column("file_url")]
    public string FileUrl { get; set; } = string.Empty;

    [Column("file_size")]
    public long? FileSize { get; set; }

    [Column("mime_type")]
    public string? MimeType { get; set; }

    [Column("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [Column("uploaded_by_id")]
    public Guid UploadedById { get; set; }

    [Column("uploaded_at")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

}
