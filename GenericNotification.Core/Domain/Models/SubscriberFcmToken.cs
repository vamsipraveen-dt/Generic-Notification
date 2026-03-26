namespace GenericNotification.Core.Domain.Models;

public partial class SubscriberFcmToken
{
    public int SubscriberFcmTokenId { get; set; }

    public string? SubscriberUid { get; set; }

    public string? FcmToken { get; set; }

    public string? CreatedDate { get; set; }

    public virtual Subscriber? SubscriberU { get; set; }
}
