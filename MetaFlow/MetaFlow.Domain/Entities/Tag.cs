using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("tags")]
public class Tag : BaseEntity
{
    [Column("board_id")]
    public Guid BoardId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("color")]
    public string Color { get; set; } = "#808080";

    [Column("description")]
    public string? Description { get; set; }

    [Column("usage_count")]
    public int UsageCount { get; set; }

}
