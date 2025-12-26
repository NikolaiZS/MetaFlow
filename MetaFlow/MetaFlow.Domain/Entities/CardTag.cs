using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class CardTag
    {
        public Guid CardId { get; set; }
        public Guid TagId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public Guid? AddedBy { get; set; }

        public Card Card { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }

}
