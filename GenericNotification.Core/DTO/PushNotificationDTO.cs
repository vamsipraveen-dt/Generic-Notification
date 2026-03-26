namespace GenericNotification.Core.DTO
{
    public class PushNotificationDTO
    {
        public string? AccessToken { get; set; }
        public required string Suid { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required string Text { get; set; }
        public string? Context { get; set; }
        public string? Url { get; set; }
    }
}
