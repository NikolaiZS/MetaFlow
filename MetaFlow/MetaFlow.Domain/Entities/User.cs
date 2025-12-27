using Supabase.Postgrest.Attributes;
using MetaFlow.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaFlow.Domain.Entities;

[Table("users")]
public class User : BaseEntity
{
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("full_name")]
    public string? FullName { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("email_verified")]
    public bool EmailVerified { get; set; }

    [Column("preferences")]
    public string Preferences { get; set; } = "{}";

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

}
