namespace MetaFlow.Domain.Models
{
    public class CommentMetadata
    {
        public bool IsSystemGenerated { get; set; } = false;
        public string? EventType { get; set; }
        public Dictionary<string, object> EventData { get; set; } = new();
    }
}