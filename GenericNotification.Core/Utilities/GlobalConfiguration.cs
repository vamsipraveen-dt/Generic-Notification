using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericNotification.Core.Utilities
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        private readonly ILogger<GlobalConfiguration> _logger;
        private readonly IConfigurationService _configurationService;

        private string? _smtpConfiguration;
        private string? _fcmConfiguration;

        private bool _initialized = false;

        // Ensures thread-safe, single initialization
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public GlobalConfiguration(
            ILogger<GlobalConfiguration> logger,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_initialized)
                    return;

                _logger.LogInformation("Initializing Global Configuration");

                // FCM
                var fcmObject = await _configurationService
                    .GetConfigurationAsync<JObject>("FCM_Config");

                if (fcmObject == null)
                    throw new InvalidOperationException("FCM_Config not found in database");

                _fcmConfiguration = JsonConvert.SerializeObject(fcmObject);

                // SMTP
                var smtpObject = await _configurationService
                    .GetConfigurationAsync<JObject>("SMTP");

                if (smtpObject == null)
                    throw new InvalidOperationException("SMTP configuration not found in database");

                _smtpConfiguration = JsonConvert.SerializeObject(smtpObject);

                _initialized = true;

                _logger.LogInformation("Global Configuration initialized successfully");
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<string> GetFCMConfigurationAsync()
        {
            await EnsureInitializedAsync();
            return _fcmConfiguration!;
        }

        public async Task<SmtpDTO?> GetSMTPConfigurationAsync()
        {
            await EnsureInitializedAsync();
            return JsonConvert.DeserializeObject<SmtpDTO>(_smtpConfiguration!);
        }
    }
}
