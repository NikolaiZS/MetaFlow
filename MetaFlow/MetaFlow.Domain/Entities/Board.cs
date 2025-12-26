using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class Board : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid MethodologyPresetId { get; set; }
        public string CustomConfig { get; set; } = "{}"; 
        public Guid OwnerId { get; set; }
        public bool IsPublic { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }

        public User Owner { get; set; } = null!;
        public MethodologyPreset MethodologyPreset { get; set; } = null!;
        public ICollection<Column> Columns { get; set; } = new List<Column>();
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
        public ICollection<Swimlane> Swimlanes { get; set; } = new List<Swimlane>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    }
}
