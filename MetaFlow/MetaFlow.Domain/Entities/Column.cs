using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class Column : BaseEntity
    {
        public Guid BoardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Position { get; set; }
        public string ColumnType { get; set; } = "standard";
        public int? WipLimit { get; set; }
        public string Color { get; set; } = "#e0e0e0";
        public string Settings { get; set; } = "{}"; 
        public bool IsVisible { get; set; } = true;

        public Board Board { get; set; } = null!;
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }

}
