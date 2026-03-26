using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace GenericNotification.API.Controllers
{
    [Route("api/sms")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost]
        [Route("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SmsDTO smsDTO)
        {
            var result = await _smsService.SendSmsAsync(smsDTO);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }

    }
}
