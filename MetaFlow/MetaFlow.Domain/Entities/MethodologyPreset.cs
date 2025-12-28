using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using MetaFlow.Domain.Models;

namespace MetaFlow.Domain.Entities;

[Table("methodology_presets")]
public class MethodologyPreset : BaseEntity
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("icon")]
    public string? Icon { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("config")]
    public MethodologyConfig Config { get; set; } = new();

    [Column("is_system")]
    public bool IsSystem { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_by")]
    public Guid? CreatedBy { get; set; }
}
