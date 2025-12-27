using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("card_tags")]
public class CardTag : BaseModel
{
    [Column("card_id")]
    public Guid CardId { get; set; }

    [Column("tag_id")]
    public Guid TagId { get; set; }

    [Column("added_at")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    [Column("added_by")]
    public Guid? AddedBy { get; set; }

}
