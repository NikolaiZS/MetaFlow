using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using Newtonsoft.Json;

namespace MetaFlow.Domain.Entities;

[Table("board_members")]
public class BoardMember : BaseEntity
{
    [Column("board_id")]
    public Guid BoardId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("role")]
    public string Role { get; set; } = "member";

    [JsonIgnore]
    public string? Permissions { get; set; }

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [Column("invited_by")]
    public Guid? InvitedBy { get; set; }
}
