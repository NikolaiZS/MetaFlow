using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using Newtonsoft.Json;

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

    [JsonIgnore]
    public string CustomConfig { get; set; } = "{}";

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
