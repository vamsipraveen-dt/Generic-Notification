using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;

namespace GenericNotification.Core.Domain.Services
{
    public interface IPushNotificationService
    {
        Task<ServiceResult> SendAuthnNotification(AuthnNotificationDTO request);
        Task<ServiceResult> SendEConsentNotification(EConsentNotificationDTO request);
        Task<ServiceResult> SendNotification(PushNotificationDTO request);
        Task<ServiceResult> SendNotificationDelegationRequest(DelegationPushNotificationDTO request);
    }
}
