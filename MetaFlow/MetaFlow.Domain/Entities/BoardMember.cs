using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using MetaFlow.Domain.Models;

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

    [Column("permissions")]
    public MemberPermissions Permissions { get; set; } = new();

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [Column("invited_by")]
    public Guid? InvitedBy { get; set; }
}
