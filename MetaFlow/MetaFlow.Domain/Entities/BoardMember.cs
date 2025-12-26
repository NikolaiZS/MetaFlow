using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class BoardMember : BaseEntity
    {
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = "member";
        public string Permissions { get; set; } = "{}";
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public Guid? InvitedBy { get; set; }

        public Board Board { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
