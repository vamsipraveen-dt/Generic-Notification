using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace GenericNotification.API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost]
        [Route("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDTO request)
        {
            if (request == null)
            {
                return Ok(new APIResponse() { Result = null, Message = "Request body should not be null", Success = false });
            }
            var result = await _emailService.SendEmailAsync(request);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }

        [HttpPost]
        [Route("send-email-attachment")]
        public async Task<IActionResult> SendEmailWithAttachment([FromForm] SendEmailRequestDTO request, string filename)
        {
            if (request == null)
            {
                return Ok(new APIResponse() { Result = null, Message = "Request body should not be null", Success = false });
            }
            if (request.Attachment == null || request.Attachment.Length == 0)
            {
                return Ok(new APIResponse() { Result = null, Message = "Attachment should not be null", Success = false });
            }
            if (string.IsNullOrEmpty(filename))
            {
                return Ok(new APIResponse() { Result = null, Message = "Filename should not be null", Success = false });
            }

            var result = await _emailService.SendEmailWithAttachmentAsync(request, filename);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }
    }
}
