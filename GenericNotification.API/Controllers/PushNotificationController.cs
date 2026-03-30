using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace GenericNotification.API.Controllers
{
    [Route("api/push-notification")]
    [ApiController]
    [AllowAnonymous]
    public class PushNotificationController : ControllerBase
    {
        IPushNotificationService _pushNotificationService;

        public PushNotificationController(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost]
        [Route("send-push-notification")]
        public async Task<IActionResult> SendPushNotification([FromBody] PushNotificationDTO request)
        {
            if (request == null)
            {
                return Ok(new APIResponse() { Result = null, Message = "Request body should not be null", Success = false });
            }

            var result = await _pushNotificationService.SendNotification(request);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }

        [HttpPost]
        [Route("send-authn-notification")]
        public async Task<IActionResult> SendAuthnNotification([FromBody] AuthnNotificationDTO request)
        {
            if (request == null)
            {
                return Ok(new APIResponse() { Result = null, Message = "Request body should not be null", Success = false });
            }
            var result = await _pushNotificationService.SendAuthnNotification(request);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }

        [HttpPost]
        [Route("send-econsent-notification")]
        public async Task<IActionResult> SendEConsentNotification([FromBody] EConsentNotificationDTO request)
        {
            if (request == null)
            {
                return Ok(new APIResponse() { Result = null, Message = "Request body should not be null", Success = false });
            }
            var result = await _pushNotificationService.SendEConsentNotification(request);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }

        [HttpPost]
        [Route("send-delegation-notification")]
        public async Task<IActionResult> SendDelegationNotification([FromBody] DelegationPushNotificationDTO request)
        {
            if (request == null)
            {
                return Ok(new APIResponse() { Result = null, Message = "Request body should not be null", Success = false });
            }
            var result = await _pushNotificationService.SendNotificationDelegationRequest(request);

            return Ok(new APIResponse() { Result = result.Result, Message = result.Message, Success = result.Success });
        }
    }
}
