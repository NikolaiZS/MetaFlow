namespace MetaFlow.Domain.Models
{
    public class HistoryChanges
    {
        public Dictionary<string, object> Before { get; set; } = new();
        public Dictionary<string, object> After { get; set; } = new();
        public List<string> ModifiedFields { get; set; } = new();
    }
}