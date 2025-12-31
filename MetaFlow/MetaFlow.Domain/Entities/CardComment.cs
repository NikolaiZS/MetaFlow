using MetaFlow.Domain.Common;
using MetaFlow.Domain.Models;
using Supabase.Postgrest.Attributes;

namespace MetaFlow.Domain.Entities;

[Table("card_comments")]
public class CardComment : BaseEntity
{
    [Column("card_id")]
    public Guid CardId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("parent_comment_id")]
    public Guid? ParentCommentId { get; set; }

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("metadata")]
    public CommentMetadata Metadata { get; set; } = new();

    [Column("is_edited")]
    public bool IsEdited { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}