using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;

namespace GenericNotification.Core.Domain.Services
{
    public interface IEmailService
    {
        Task<ServiceResult> SendEmailAsync(SendEmailRequestDTO message);
        Task<ServiceResult> SendEmailWithAttachmentAsync(SendEmailRequestDTO message, string fileName);
    }
}
