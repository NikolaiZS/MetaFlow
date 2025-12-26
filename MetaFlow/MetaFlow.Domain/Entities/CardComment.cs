using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class CardComment : BaseEntity
    {
        public Guid CardId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Metadata { get; set; } = "{}";
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Card Card { get; set; } = null!;
        public User User { get; set; } = null!;
        public CardComment? ParentComment { get; set; }
    }

}
