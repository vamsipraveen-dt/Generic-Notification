namespace GenericNotification.Core.Domain.Models;

public partial class Subscriber
{
    public int SubscriberId { get; set; }

    public string SubscriberUid { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string? DateOfBirth { get; set; }

    public string? IdDocType { get; set; }

    public string? IdDocNumber { get; set; }

    public string? NationalId { get; set; }

    public string CreatedDate { get; set; } = null!;

    public string UpdatedDate { get; set; } = null!;

    public string OsName { get; set; } = null!;

    public string OsVersion { get; set; } = null!;

    public string AppVersion { get; set; } = null!;

    public string DeviceInfo { get; set; } = null!;

    public short IsSmartphoneUser { get; set; }

    public string? Title { get; set; }
    public virtual SubscriberFcmToken? SubscriberFcmToken { get; set; }

}
