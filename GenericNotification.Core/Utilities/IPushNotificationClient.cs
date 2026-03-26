using GenericNotification.Core.Domain.Services.Communication.Notification;
using GenericNotification.Core.DTO;

namespace GenericNotification.Core.Utilities
{
    public interface IPushNotificationClient
    {
        Task<string?> SendAuthnNotification(AuthnNotificationDTO authnNotification);
        Task<string?> SendEConsentNotification(EConsentNotificationDTO eConsentNotification);
        Task<string?> SendNotification(PushNotificationRequest request);
    }
}