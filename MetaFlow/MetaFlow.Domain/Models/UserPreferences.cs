namespace MetaFlow.Domain.Models
{
    public class UserPreferences
    {
        public string Theme { get; set; } = "light";
        public string Language { get; set; } = "en";
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = false;
        public int ItemsPerPage { get; set; } = 20;
        public string DateFormat { get; set; } = "YYYY-MM-DD";
        public string TimeZone { get; set; } = "UTC";
    }
}