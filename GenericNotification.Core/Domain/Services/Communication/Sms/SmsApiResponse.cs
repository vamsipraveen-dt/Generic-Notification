namespace GenericNotification.Core.Domain.Services.Communication.Sms
{
    public class SmsApiResponse
    {
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? JobId { get; set; }
        public List<MessageData>? MessageData { get; set; }
    }

    public class MessageData
    {
        public string? Number { get; set; }
        public string? MessageId { get; set; }
        public string? Message { get; set; }
    }

}
