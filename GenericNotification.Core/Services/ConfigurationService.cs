using GenericNotification.Core.Domain.Repositories;
using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GenericNotification.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        // Initialize logger.
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(IUnitOfWork unitOfWork,
            ILogger<ConfigurationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public T? GetPlainConfiguration<T>(string configName)
        {
            _logger.LogDebug("-->GetConfiguration");

            // Get Configuration Record
            var configRecord = _unitOfWork.Configuration.GetConfigurationByName(configName);
            if (configRecord == null || null == configRecord.Value)
            {
                _logger.LogError("Get Plain Configuration Record Failed");
                return default;
            }


            // Convert Plain data string to object
            T? config = JsonConvert.DeserializeObject<T>(configRecord.Value);
            if (EqualityComparer<T>.Default.Equals(config, default))
            {
                _logger.LogError("Convert Plain data string to object Failed");
                return default;
            }

            _logger.LogDebug("<--GetConfiguration");
            return config;
        }
        public T? GetConfiguration<T>(string configName)
        {
            _logger.LogDebug("-->GetConfiguration");

            // Get Configuration Record
            var configRecord = _unitOfWork.Configuration.GetConfigurationByName(configName);
            if (null == configRecord || configRecord.Value==null)
            {
                _logger.LogError("Get Configuration Record Failed");
                return default;
            }

            // Get Plain data from secured data
            var plainData = PKIMethods.Instance.
                PKIDecryptSecureWireData(configRecord.Value);
            if (null == plainData)
            {
                _logger.LogError("PKIDecryptSecureWireData Failed");
                return default;
            }
            T? config = JsonConvert.DeserializeObject<T>(plainData);
            // Convert Plain data string to object
            if (EqualityComparer<T>.Default.Equals(config, default))
            {
                _logger.LogError("Convert Plain data string to object Failed");
                return default;
            }

            _logger.LogDebug("<--GetConfiguration");
            return config;
        }

        public async Task<T?> GetConfigurationAsync<T>(string configName)
        {
            _logger.LogDebug("-->GetConfiguration");

            // Get Configuration Record
            var configRecord = await _unitOfWork.Configuration.GetConfigurationByNameAsync(configName);
            if (null == configRecord || null == configRecord.Value)
            {
                _logger.LogError("Get Configuration Record Async Failed");
                return default;
            }

            // Get Plain data from secured data
            var plainData = PKIMethods.Instance.
                PKIDecryptSecureWireData(configRecord.Value);
            if (null == plainData)
            {
                _logger.LogError("PKIDecryptSecureWireData Failed");
                return default;
            }

            // Convert Plain data string to object
            T? config = JsonConvert.DeserializeObject<T>(plainData);
            if (EqualityComparer<T>.Default.Equals(config, default))
            {
                _logger.LogError("Convert Plain data string to object Failed");
                return default;
            }

            _logger.LogDebug("<--GetConfiguration");
            return config;
        }

        public ConfigurationResponse? SetConfiguration(
                string configName, object config)
        {
            _logger.LogDebug("-->SetConfiguration");

            // Get Configuration Record
            var configRecord = _unitOfWork.Configuration.GetConfigurationByName(configName);
            if (null == configRecord || null == configRecord.Value)
            {
                _logger.LogError("Get Configuration Record Failed");
                return null;
            }

            // Convert Configuration Object to string
            var serializedConfig = JsonConvert.SerializeObject(config);
            if (null == serializedConfig)
            {
                _logger.LogError("Convert Configuration Object to string Failed");
                return null;
            }

            // Create Secure data from plain data
            var secureData = PKIMethods.Instance.
                PKICreateSecureWireData(serializedConfig);
            if (null == secureData)
            {
                _logger.LogError("PKICreateSecureWireData Failed");
                return null;
            }

            // Keep the updated data
            configRecord.Value = secureData;

            try
            {
                _unitOfWork.Configuration.Update(configRecord);
                _unitOfWork.Save();
                return new ConfigurationResponse(configRecord);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ConfigurationResponse?> SetConfigurationAsync(
                string configName, object config, string updatedBy)
        {
            _logger.LogDebug("-->SetConfiguration");
            var secureData = string.Empty;

            // Get Configuration Record
            var configRecord = await _unitOfWork.Configuration.GetConfigurationByNameAsync(configName);
            if (configRecord == null || null == configRecord.Value)
            {
                _logger.LogError("Get Configuration Record Failed");
                return null;
            }

            try
            {
                // Convert Configuration Object to string
                var serializedConfig = JsonConvert.SerializeObject(config);
                if (null == serializedConfig)
                {
                    _logger.LogError("Convert Configuration Object to string Failed");
                    return null;
                }

                // Create Secure data from plain data
                secureData = PKIMethods.Instance.
                    PKICreateSecureWireData(serializedConfig);
                if (null == secureData)
                {
                    _logger.LogError("PKICreateSecureWireData Failed");
                    return null;
                }

                // Keep the updated data
                configRecord.Value = secureData;
                configRecord.UpdatedBy = updatedBy;
                configRecord.ModifiedDate = DateTime.Now;

                _unitOfWork.Configuration.Update(configRecord);
                await _unitOfWork.SaveAsync();
                return new ConfigurationResponse(configRecord, "Configuration updated successfully");
            }
            catch
            {
                return null;
            }
        }
    }
}
