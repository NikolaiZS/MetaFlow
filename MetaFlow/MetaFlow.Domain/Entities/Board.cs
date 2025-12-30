using MetaFlow.Domain.Common;
using MetaFlow.Domain.Models;
using Supabase.Postgrest.Attributes;

namespace MetaFlow.Domain.Entities;

[Table("boards")]
public class Board : BaseEntity
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("methodology_preset_id")]
    public Guid MethodologyPresetId { get; set; }

    [Column("custom_config")]
    public BoardConfig CustomConfig { get; set; } = new();

    [Column("owner_id")]
    public Guid OwnerId { get; set; }

    [Column("is_public")]
    public bool IsPublic { get; set; }

    [Column("is_template")]
    public bool IsTemplate { get; set; }

    [Column("is_archived")]
    public bool IsArchived { get; set; }

    [Column("archived_at")]
    public DateTime? ArchivedAt { get; set; }
}