using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;

namespace GenericNotification.Core.Domain.Services
{
    public interface ISmsService
    {
        Task<ServiceResult> SendSmsAsync(SmsDTO smsDTO);

    }
}
