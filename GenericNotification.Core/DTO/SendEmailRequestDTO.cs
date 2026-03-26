using Microsoft.AspNetCore.Http;

namespace GenericNotification.Core.DTO
{
    public class SendEmailRequestDTO
    {
        public required List<string> To { get; set; }
        public required string Subject { get; set; }
        public string? Content { get; set; }

        public IFormFile? Attachment { get; set; }
        public bool IsAttachmentPresent { get; set; } = false;
    }
}
