using MimeKit;

namespace GenericNotification.Core.DTOs
{
    public class SmtpDTO
    {
        public required string Server { get; set; }
        public int Port { get; set; }
        public required string SenderUser { get; set; }
        public required string Password { get; set; }

    }


    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public byte[]? Attachment { get; set; }

        public bool IsAttachmentPresent { get; set; } = false;

        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();

            To = to.Select(x => new MailboxAddress("", x)).ToList();
            Subject = subject;
            Content = content;
        }

        public Message(IEnumerable<string> to, string subject, string content, byte[] attachment, bool isAttachmentPresent)
        {
            To = new List<MailboxAddress>();

            To = to.Select(x => new MailboxAddress("", x)).ToList();
            Subject = subject;
            Content = content;
            Attachment = attachment;
            IsAttachmentPresent = isAttachmentPresent;
        }
    }
    public class EmailConfiguration
    {
        public string? From { get; set; }
        public required string SmtpServer { get; set; }
        public required int Port { get; set; }
        public required string UserName { get; set; }
        public string? Password { get; set; }
    }

    public class SendEmailObj
    {
        public string? Id { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
    }
    public class SmtpSettings
    {
        public required string SmtpHost { get; set; }
        public required string FromName { get; set; }
        public required string FromEmailAddr { get; set; }
        public bool RequireAuth { get; set; }
        public required string SmtpUserName { get; set; }
        public required string SmtpPwd { get; set; }
        public bool RequiresSsl { get; set; }
        public int SmtpPort { get; set; }
        public string? MailSubject { get; set; }
        public string? Template { get; set; }
        public int Id { get; set; }
    }
}
