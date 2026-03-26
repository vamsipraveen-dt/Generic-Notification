using GenericNotification.Core.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace GenericNotification.Core.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGlobalConfiguration _globalConfiguration;
        private SmtpDTO? SMTP;
        private bool _configsLoaded = false;
        public EmailSender(ILogger<EmailSender> logger,
            IConfiguration configuration,
            IGlobalConfiguration globalConfiguration
            )
        {
            _configuration = configuration;
            _logger = logger;
            _globalConfiguration = globalConfiguration;

        }

        private async Task EnsureConfigsLoadedAsync()
        {
            if (_configsLoaded)
                return;

            SMTP = await _globalConfiguration.GetSMTPConfigurationAsync();

            if (SMTP == null)
            {
                _logger.LogError("SMTP configuration is missing");
                throw new InvalidOperationException("SMTP configuration not loaded");
            }

            _configsLoaded = true;
        }


        private async Task<MimeMessage?> CreateEmailMessage(Message message)
        {
            _logger.LogDebug("-->CreateEmailMessage");

            await EnsureConfigsLoadedAsync();

            // Validate Input Parameters
            if (null == message)
            {
                _logger.LogError("Invalid Input Parameter");
                return null;
            }
            var emailMessage = new MimeMessage();

            try
            {
                emailMessage.From.Add(new MailboxAddress
                      (null, SMTP?.SenderUser));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                var html = message.Content
       .Replace("\r\n", "<br>")
       .Replace("\n", "<br>");

                emailMessage.Body = new TextPart(TextFormat.Html)
                {
                    Text = html
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateEmailMessage Failed : {0}", ex.Message);
                return null;
                throw;
            }

            _logger.LogDebug("<--CreateEmailMessage");
            return emailMessage;
        }

        private async Task<bool> Send(MimeMessage mailMessage)
        {
            _logger.LogDebug("-->Send");

            await EnsureConfigsLoadedAsync();
            bool result = false;

            if (null == mailMessage)
            {
                _logger.LogError("Invalid Input Parameter");
                return result;
            }

            using (var client = new SmtpClient())
            {
                try
                {
                    string? DecryptedPasswd = null;

                    if (!string.IsNullOrWhiteSpace(SMTP!.Password))
                    {
                        DecryptedPasswd = PKIMethods.Instance.PKIDecryptSecureWireData(SMTP.Password);

                        if (string.IsNullOrWhiteSpace(DecryptedPasswd))
                        {
                            _logger.LogError("Failed to decrypt SMTP password");
                            return false;
                        }
                    }
                    var mailConfig = new EmailConfiguration() {
                        SmtpServer = SMTP.Server,
                        Port = SMTP.Port,
                        UserName = SMTP.SenderUser,
                        Password = DecryptedPasswd
                    };
                    SecureSocketOptions socketOptions = SecureSocketOptions.None;

                    switch (mailConfig.Port)
                    {
                        case 465:
                            socketOptions = SecureSocketOptions.SslOnConnect;
                            break;

                        case 587:
                            socketOptions = SecureSocketOptions.StartTls;
                            break;

                        case 25:
                            socketOptions = SecureSocketOptions.None;
                            break;
                    }

                    await client.ConnectAsync(mailConfig.SmtpServer, mailConfig.Port, socketOptions);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    if (mailConfig.Port != 25 && !string.IsNullOrWhiteSpace(mailConfig.UserName) &&
    !string.IsNullOrWhiteSpace(mailConfig.Password))
                    {
                        await client.AuthenticateAsync(mailConfig.UserName, mailConfig.Password);
                    }

                    await client.SendAsync(mailMessage);
                    result = true;

                }
                catch (Exception error)
                {
                    _logger.LogError(
                error,
                "Failed to send mail to {Recipients} via {SmtpServer}:{Port}",
                string.Join(", ", mailMessage.To),
                SMTP?.Server,
                SMTP?.Port);
                }
                finally
                {
                   await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }

            _logger.LogDebug("<--Send");
            return result;
        }

        public async Task<bool> TestSmtpConnection()
        {
            _logger.LogInformation("-->TestSmtpConnection");

            await EnsureConfigsLoadedAsync();

            bool result = false;


            using (var client = new SmtpClient())
            {
                try
                {
                    if (SMTP?.Port == 465)
                    {
                       await client.ConnectAsync(SMTP.Server,
                        SMTP.Port, true);
                    }
                    else if (SMTP?.Port == 587)
                    {
                       await client.ConnectAsync(SMTP.Server,
                        SMTP.Port, false);
                    }
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                   await client.AuthenticateAsync(SMTP?.SenderUser,
                        SMTP?.Password);
                    result = true;
                }
                catch (Exception error)
                {
                    _logger.LogError(error,"Failed to TestSmtpConnection: {0}",
                        error.Message);

                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }

            _logger.LogInformation("<--TestSmtpConnection");
            return result;
        }

        public async Task<bool> SendEmail(Message message)
        {
            _logger.LogInformation("-->SendEmail");

            await EnsureConfigsLoadedAsync();

            bool result = false;

            // Validate Input Parameters
            if (null == message)
            {
                _logger.LogError("Invalid Input Parameter");
                return result;
            }

            var emailMessage = await CreateEmailMessage(message);
            if (null == emailMessage)
            {
                _logger.LogError("CreateEmailMessage Failed");
                return result;
            }

            // Send Email
            result = await Send(emailMessage);
            if (!result)
            {
                _logger.LogError("Send Email Failed");
                return result;
            }

            //Return Success
            result = true;

            _logger.LogDebug("<--SendEmail");
            return result;
        }

        public async Task<bool> SendEmailWithAttachment(Message message, string fileName)
        {
            _logger.LogInformation("-->SendEmailWithAttachment");

            await EnsureConfigsLoadedAsync();

            bool result = false;

            // Validate Input Parameters
            if (null == message)
            {
                _logger.LogError("Invalid Input Parameter");
                return result;
            }

            var emailMessage = await CreateEmailMessageWithAttachment(message, fileName);
            if (null == emailMessage)
            {
                _logger.LogError("CreateEmailMessage Failed");
                return result;
            }

            // Send Email
            result = await Send(emailMessage);
            if (!result)
            {
                _logger.LogError("Send Email With Attachment Failed");
                _logger.LogError("Send Email With Attachment Failed to email : " + message.To[0]);
                return result;
            }

            foreach (var email in message.To)
            {
                _logger.LogInformation(
    "Send Email With Attachment to email success: {Email}",
    email);
            }
            //Return Success
            result = true;

            _logger.LogInformation("<--SendEmailWithAttachment");
            return result;
        }

        private async Task<MimeMessage?> CreateEmailMessageWithAttachment(Message message, string fileName)
        {
            _logger.LogInformation("-->CreateEmailMessageWithAttachment");

            await EnsureConfigsLoadedAsync();

            // Validate Input Parameters
            if (null == message)
            {
                _logger.LogError("Invalid Input Parameter");
                return null;
            }
            var emailMessage = new MimeMessage();

            try
            {
                emailMessage.From.Add(new MailboxAddress
                      (null, SMTP?.SenderUser));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message.Content
                };

                if (message.IsAttachmentPresent)
                {
                    var builder = new BodyBuilder();
                    if (message.Attachment != null)
                    {
                        var file = message.Attachment;
                        if (file.Length > 0)
                        {
                            //using (var ms = new MemoryStream())
                            //{
                            //    file.CopyTo(ms);
                            //    fileBytes = ms.ToArray();
                            //}
                            string mimeType = MimeTypes.GetMimeType(fileName); // Auto-detect MIME type
                            builder.Attachments.Add(fileName, message.Attachment, MimeKit.ContentType.Parse(mimeType));
                            //builder.Attachments.Add(fileName, message.Attachment, MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));
                        }
                    }
                    builder.HtmlBody = message.Content;
                    emailMessage.Body = builder.ToMessageBody();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
        ex,
        "CreateEmailMessageWithAttachment failed");

                return null;
            }

            _logger.LogInformation("<--CreateEmailMessageWithAttachment");
            return emailMessage;
        }
    }
}
