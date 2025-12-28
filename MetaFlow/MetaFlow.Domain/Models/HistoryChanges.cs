using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Models
{
    public class HistoryChanges
    {
        public Dictionary<string, object> Before { get; set; } = new();
        public Dictionary<string, object> After { get; set; } = new();
        public List<string> ModifiedFields { get; set; } = new();
    }
}
