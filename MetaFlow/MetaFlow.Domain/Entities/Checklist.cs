using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("checklists")]
public class Checklist : BaseEntity
{
    [Column("card_id")]
    public Guid CardId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("position")]
    public int Position { get; set; }

}
