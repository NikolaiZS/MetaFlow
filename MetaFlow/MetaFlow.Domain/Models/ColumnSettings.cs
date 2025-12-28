using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Models
{
    public class ColumnSettings
    {
        public bool AutoProgress { get; set; } = false;
        public List<string> AllowedCardTypes { get; set; } = new();
        public int? MaxCards { get; set; }
        public bool CollapsedByDefault { get; set; } = false;
        public Dictionary<string, object> Automation { get; set; } = new();
    }
}
