using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class ChecklistItem : BaseEntity
    {
        public Guid ChecklistId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Position { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid? CompletedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }

        public Checklist Checklist { get; set; } = null!;
    }

}
