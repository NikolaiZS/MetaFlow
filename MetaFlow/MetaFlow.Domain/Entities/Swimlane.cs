using MetaFlow.Domain.Common;
using Supabase.Postgrest.Attributes;

namespace MetaFlow.Domain.Entities;

[Table("swimlanes")]
public class Swimlane : BaseEntity
{
    [Column("board_id")]
    public Guid BoardId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("position")]
    public int Position { get; set; }

    [Column("swimlane_type")]
    public string SwimlaneType { get; set; } = "custom";

    [Column("color")]
    public string? Color { get; set; }

    [Column("is_collapsed")]
    public bool IsCollapsed { get; set; }

    [Column("is_visible")]
    public bool IsVisible { get; set; } = true;
}