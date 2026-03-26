using GenericNotification.Core.DTOs;

namespace GenericNotification.Core.Utilities
{
    public interface IGlobalConfiguration
    {
        Task<string> GetFCMConfigurationAsync();
        Task<SmtpDTO?> GetSMTPConfigurationAsync();
    }
}