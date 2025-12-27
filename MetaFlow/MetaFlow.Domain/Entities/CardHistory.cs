using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace MetaFlow.Domain.Entities;

[Table("card_history")]
public class CardHistory : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("card_id")]
    public Guid CardId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("action")]
    public string Action { get; set; } = string.Empty;

    [Column("from_column_id")]
    public Guid? FromColumnId { get; set; }

    [Column("to_column_id")]
    public Guid? ToColumnId { get; set; }

    [JsonIgnore]
    public string? Changes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
