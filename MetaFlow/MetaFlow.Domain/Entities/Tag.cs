using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public Guid BoardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#808080";
        public string? Description { get; set; }
        public int UsageCount { get; set; }

        public Board Board { get; set; } = null!;
        public ICollection<CardTag> CardTags { get; set; } = new List<CardTag>();
    }

}
