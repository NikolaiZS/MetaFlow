using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class MethodologyPreset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Category { get; set; }
        public string Config { get; set; } = "{}";
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }

        public ICollection<Board> Boards { get; set; } = new List<Board>();
    }

}
