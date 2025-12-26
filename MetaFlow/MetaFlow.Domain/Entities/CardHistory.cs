using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class CardHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CardId { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public Guid? FromColumnId { get; set; }
        public Guid? ToColumnId { get; set; }
        public string? Changes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Card Card { get; set; } = null!;
    }

}
