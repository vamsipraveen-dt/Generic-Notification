using GenericNotification.Core.Domain.Services.Communication.Notification;

namespace GenericNotification.Core.DTO
{
    public class AuthnNotificationDTO
    {
        public required string RegistrationToken { get; set; }
        public required string AuthnToken { get; set; }
        public required string AuthnScheme { get; set; }
        public required string RandomCodes { get; set; }
        public required string ApplicationName { get; set; }
        public required string DeviceName { get; set; }
        public required string TimeStamp { get; set; }
        public string? Language { get; set; } = "en";
    }

    public class EConsentNotificationDTO
    {
        public required string RegistrationToken { get; set; }
        public required string AuthnToken { get; set; }
        public required string AuthnScheme { get; set; }
        public required string ApplicationName { get; set; }
        public List<ScopeInfo>? ConsentScopes { get; set; }
        public bool DeselectScopesAndClaims { get; set; }
    }
}
