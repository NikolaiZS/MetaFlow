using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Models
{
    public class CommentMetadata
    {
        public bool IsSystemGenerated { get; set; } = false;
        public string? EventType { get; set; }
        public Dictionary<string, object> EventData { get; set; } = new();
    }
}
