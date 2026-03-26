namespace GenericNotification.Core.Domain.Services.Communication
{
    public class Accesstoken
    {
        public string? GlobalSessionId { get; set; }
        public string? UserId { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresAt { get; set; }
        public string? ClientId { get; set; }
        public string? Scopes { get; set; }
        public string? RefreshToken { get; set; }
        public string? RefreshTokenExpiresAt { get; set; }
        public string? GrantType { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
