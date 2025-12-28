using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Models
{
    public class MethodologyConfig
    {
        public List<string> DefaultColumns { get; set; } = new();
        public int? WipLimit { get; set; }
        public bool AllowSwimlanes { get; set; } = false;
        public List<string> RequiredFields { get; set; } = new();
        public Dictionary<string, object> CustomRules { get; set; } = new();
        public bool EnableSprints { get; set; } = false;
        public int DefaultSprintDuration { get; set; } = 14;
    }
}
