using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace GenericNotification.Core.Services
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmsService> _logger;
        private readonly IConfiguration _configuration;

        public SmsService(HttpClient httpClient, ILogger<SmsService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResult> SendSmsAsync(SmsDTO smsDTO)
        {
            try
            {
                var url = _configuration["SmsConfig:Url"];
                _logger.LogInformation($"Sending SMS to : {smsDTO.MobileNo} >> SMS Body : {smsDTO.SmsText}");
                var requestBody = new
                {
                    mobileno = smsDTO.MobileNo,
                    smstext = smsDTO.SmsText
                };

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"SMS sent successfully to number : {smsDTO.MobileNo}");
                    return new ServiceResult(responseBody, "SMS Sent Successfully");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogWarning($"Failed to send SMS to : {smsDTO.MobileNo}, Something went wrong >> Error : {response.StatusCode}");
                    return new ServiceResult($"Failed to send SMS to : {smsDTO.MobileNo}, Something went wrong >> Error : {response.StatusCode}");
                }
                else
                {
                    _logger.LogError($"Failed to send SMS to : {smsDTO.MobileNo} >> Error : {response.StatusCode}");
                    return new ServiceResult($"Failed to send SMS to : {smsDTO.MobileNo} >> Error : {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occurred while sending SMS to : {smsDTO.MobileNo} >> Error : {ex.Message},{ex}");
                return new ServiceResult($"Error occurred while sending SMS to : {smsDTO.MobileNo}, Error : {ex.Message}, {ex} ");
            }
        }

    }
}
