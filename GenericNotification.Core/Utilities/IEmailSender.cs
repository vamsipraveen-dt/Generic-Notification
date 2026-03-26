using GenericNotification.Core.DTOs;

namespace GenericNotification.Core.Utilities
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(Message message);

        Task<bool> SendEmailWithAttachment(Message message, string fileName);

        Task<bool> TestSmtpConnection();
    }
}
