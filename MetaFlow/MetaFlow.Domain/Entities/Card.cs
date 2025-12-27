using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("cards")]
public class Card : BaseEntity
{
    [Column("board_id")]
    public Guid BoardId { get; set; }

    [Column("column_id")]
    public Guid ColumnId { get; set; }

    [Column("swimlane_id")]
    public Guid? SwimlaneId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("position")]
    public double Position { get; set; }

    [Column("priority")]
    public string Priority { get; set; } = "medium";

    [Column("status")]
    public string Status { get; set; } = "active";

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("created_by_id")]
    public Guid CreatedById { get; set; }

    [Column("assigned_to_id")]
    public Guid? AssignedToId { get; set; }

    [Column("sprint_id")]
    public Guid? SprintId { get; set; }

    [Column("custom_fields")]
    public string CustomFields { get; set; } = "{}";

    [Column("metadata")]
    public string Metadata { get; set; } = "{}";

    [Column("is_archived")]
    public bool IsArchived { get; set; }

}
