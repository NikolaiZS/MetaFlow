using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class Checklist : BaseEntity
    {
        public Guid CardId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Position { get; set; }

        public Card Card { get; set; } = null!;
        public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
    }

}
