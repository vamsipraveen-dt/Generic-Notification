using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;
using GenericNotification.Core.DTOs;
using GenericNotification.Core.Utilities;
using Microsoft.Extensions.Logging;

namespace GenericNotification.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IEmailSender emailSender, ILogger<EmailService> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<ServiceResult> SendEmailAsync(SendEmailRequestDTO request)
        {
            if (request == null)
            {
                _logger.LogError("Validation failed: {Error}", "Message should not be null");
                return new ServiceResult("Message should not be null");
            }
            _logger.LogInformation("Sending email service method started executing...");
            try
            {
                var message = new Message(request.To, request.Subject, request.Content ?? "");
                var result = await _emailSender.SendEmail(message);
                if (!result)
                {
                    _logger.LogError("Email sending failed. Subject: {Subject}, Recipients: {Recipients}", message.Subject, string.Join(", ", message.To));
                    return new ServiceResult("Failed to send email");
                }
                _logger.LogError("Email sending success. Subject: {Subject}, Recipients: {Recipients}", message.Subject, string.Join(", ", message.To));
                return new ServiceResult(null, "Email sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email: {ErrorMessage}", ex.Message);
                return new ServiceResult("An error occurred while sending email: " + ex.Message + ", Recipients : { " + string.Join(", ", request.To) + " }");
            }
        }

        public async Task<ServiceResult> SendEmailWithAttachmentAsync(SendEmailRequestDTO request, string fileName)
        {
            if (request == null)
            {
                _logger.LogError("Validation failed: {Error}", "Message should not be null");
                return new ServiceResult("Message should not be null");
            }
            try
            {
                using var memoryStream = new MemoryStream();
                if (request.Attachment == null)
                {
                    _logger.LogInformation("No Attachment provided");
                    return new ServiceResult("Must provide attachment");

                }
                await request.Attachment.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                var message = new Message(request.To, request.Subject, request.Content ?? "", fileBytes, request.IsAttachmentPresent);
                _logger.LogInformation("Sending email with attachment service method started executing...");
                if (fileName != request.Attachment.FileName)
                {
                    string AttachmentExtension = Path.GetExtension(request.Attachment.FileName);
                    string FileExtension = Path.GetExtension(fileName);
                    if (FileExtension == "" || FileExtension != AttachmentExtension)
                    {
                        fileName = Path.GetFileNameWithoutExtension(fileName) + AttachmentExtension;
                    }

                }
                var result = await _emailSender.SendEmailWithAttachment(message, fileName);
                if (!result)
                {
                    _logger.LogError(
                        "Email sending with attachment failed. Subject: {Subject}, Recipients: {Recipients}, FileName: {FileName}",
                        message.Subject,
                        string.Join(", ", message.To),
                        fileName);
                    return new ServiceResult("Failed to send email with attachment");
                }
                _logger.LogInformation("Email sending success. Subject: {Subject}, Recipients: {Recipients}", message.Subject, string.Join(", ", message.To));
                return new ServiceResult(null, "Email sent with attachment successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email with attachment");
                return new ServiceResult("An error occurred while sending email: " + ex.Message + ", Recipients : { " + string.Join(", ", request.To) + " }");
            }
        }
    }
}
