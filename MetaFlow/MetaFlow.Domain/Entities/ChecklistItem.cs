using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("checklist_items")]
public class ChecklistItem : BaseEntity
{
    [Column("checklist_id")]
    public Guid ChecklistId { get; set; }

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("position")]
    public int Position { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("completed_by_id")]
    public Guid? CompletedById { get; set; }

    [Column("assigned_to_id")]
    public Guid? AssignedToId { get; set; }

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

}
