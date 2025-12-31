namespace MetaFlow.Domain.Models
{
    public class CardMetadata
    {
        public string? Source { get; set; }
        public string? ExternalId { get; set; }
        public Dictionary<string, string> Links { get; set; } = new();
        public List<string> Attachments { get; set; } = new();
        public Dictionary<string, object> CustomData { get; set; } = new();
    }
}