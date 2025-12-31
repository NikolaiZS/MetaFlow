using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MetaFlow.Domain.Common
{
    public abstract class BaseEntity : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}