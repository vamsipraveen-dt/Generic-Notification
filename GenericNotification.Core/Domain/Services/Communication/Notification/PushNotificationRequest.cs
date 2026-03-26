namespace GenericNotification.Core.Domain.Services.Communication.Notification
{
    public class PushNotificationRequest
    {
        public required string RegistrationToken { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public string? Text { get; set; }
        public string? Context { get; set; }
        public string? Url { get; set; }
    }
}
