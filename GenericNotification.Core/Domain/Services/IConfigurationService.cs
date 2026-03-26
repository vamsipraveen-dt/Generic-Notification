using GenericNotification.Core.Domain.Services.Communication;

namespace GenericNotification.Core.Domain.Services
{
    public interface IConfigurationService
    {
        ConfigurationResponse? SetConfiguration(
           string configName, object config);
        Task<ConfigurationResponse?> SetConfigurationAsync(
                        string configName, object config, string updatedBy);
        T? GetPlainConfiguration<T>(string configName);
        T? GetConfiguration<T>(string configName);
        Task<T?> GetConfigurationAsync<T>(string configName);
    }
}
