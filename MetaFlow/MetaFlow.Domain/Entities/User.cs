using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaFlow.Domain.Common;
using MetaFlow.Domain.Entities;

namespace MetaFlow.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PasswordHash { get; set; }
        public bool EmailVerified { get; set; }
        public string Preferences { get; set; } = "{}";
        public DateTime? LastLoginAt { get; set; }

        public ICollection<Board> OwnedBoards { get; set; } = new List<Board>();
        public ICollection<BoardMember> BoardMemberships { get; set; } = new List<BoardMember>();

    }
}
