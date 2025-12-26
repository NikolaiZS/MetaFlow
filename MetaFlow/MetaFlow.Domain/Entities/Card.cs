using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class Card : BaseEntity
    {
        public Guid BoardId { get; set; }
        public Guid ColumnId { get; set; }
        public Guid? SwimlaneId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Position { get; set; }
        public string Priority { get; set; } = "medium";
        public string Status { get; set; } = "active";
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? SprintId { get; set; }
        public string CustomFields { get; set; } = "{}";
        public string Metadata { get; set; } = "{}";
        public bool IsArchived { get; set; }

        public Board Board { get; set; } = null!;
        public Column Column { get; set; } = null!;
        public Swimlane? Swimlane { get; set; }
        public User CreatedBy { get; set; } = null!;
        public User? AssignedTo { get; set; }
        public ICollection<CardTag> CardTags { get; set; } = new List<CardTag>();
        public ICollection<CardComment> Comments { get; set; } = new List<CardComment>();
        public ICollection<CardAttachment> Attachments { get; set; } = new List<CardAttachment>();
        public ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();
        public ICollection<CardHistory> History { get; set; } = new List<CardHistory>();
    }

}
