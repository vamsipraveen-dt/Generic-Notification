using GenericNotification.Core.Domain.Models;

namespace GenericNotification.Core.Domain.Services.Communication
{
    public class ConfigurationResponse : BaseResponse<Configuration>
    {
        public ConfigurationResponse(Configuration category) : base(category) { }

        public ConfigurationResponse(string message) : base(message) { }

        public ConfigurationResponse(Configuration category, string message) :
            base(category, message)
        { }
    }

    public class ConfigurationMCRequest
    {
        public string? configName { get; set; }
        public object? requestData { get; set; }
    }
}
