using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using MetaFlow.Domain.Models;

namespace MetaFlow.Domain.Entities;

[Table("columns")]
public class Column : BaseEntity
{
    [Column("board_id")]
    public Guid BoardId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("position")]
    public int Position { get; set; }

    [Column("column_type")]
    public string ColumnType { get; set; } = "standard";

    [Column("wip_limit")]
    public int? WipLimit { get; set; }

    [Column("color")]
    public string Color { get; set; } = "#e0e0e0";

    [Column("settings")]
    public ColumnSettings Settings { get; set; } = new();

    [Column("is_visible")]
    public bool IsVisible { get; set; } = true;
}
