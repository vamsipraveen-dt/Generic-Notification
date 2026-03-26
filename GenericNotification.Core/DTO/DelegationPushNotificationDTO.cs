namespace GenericNotification.Core.DTO
{
    public class DelegationPushNotificationDTO
    {
        public string? AccessToken { get; set; }

        public List<string>? DelegateeList { get; set; }

        public required string Title { get; set; }
        public required string Body { get; set; }
        public required string ConsentData { get; set; }
        public string? Context { get; set; }
        public string? Url { get; set; }
        public bool IsDelegator { get; set; }

        public bool IsIdle { get; set; }
    }
}
