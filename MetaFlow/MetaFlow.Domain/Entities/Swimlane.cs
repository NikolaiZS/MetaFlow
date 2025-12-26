using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class Swimlane : BaseEntity
    {
        public Guid BoardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Position { get; set; }
        public string SwimlaneType { get; set; } = "custom";
        public string? Color { get; set; }
        public bool IsCollapsed { get; set; }
        public bool IsVisible { get; set; } = true;

        public Board Board { get; set; } = null!;
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }

}
