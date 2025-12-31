namespace MetaFlow.Domain.Models
{
    public class CardCustomFields
    {
        public string? StoryPoints { get; set; }
        public string? Epic { get; set; }
        public List<string> Labels { get; set; } = new();
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }
}