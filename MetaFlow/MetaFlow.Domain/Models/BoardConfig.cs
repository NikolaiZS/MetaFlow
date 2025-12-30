namespace MetaFlow.Domain.Models
{
    public class BoardConfig
    {
        public string Theme { get; set; } = "light";
        public bool ShowArchived { get; set; } = false;
        public int DefaultColumnLimit { get; set; } = 0;
        public List<string> CustomFields { get; set; } = new();
        public Dictionary<string, string> ColorScheme { get; set; } = new();
    }
}