using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using GenericNotification.Core.Domain.Services.Communication.Notification;
using GenericNotification.Core.DTO;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GenericNotification.Core.Utilities
{
    public class PushNotificationClient : IPushNotificationClient
    {
        // Initialize logger.
        private readonly ILogger<PushNotificationClient> _logger;
        private FirebaseMessaging? firebaseMessaging;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly IConfiguration _configuration;
        private bool _initialized = false;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public PushNotificationClient(
            ILogger<PushNotificationClient> logger,
            IGlobalConfiguration globalConfiguration,
            IConfiguration configuration)
        {
            _logger = logger;
            _globalConfiguration = globalConfiguration;
            _configuration = configuration;
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

                _logger.LogInformation("Initializing Firebase PushNotification client");

                var fcmConfigJson = await _globalConfiguration.GetFCMConfigurationAsync();

                FirebaseApp app;
                if (FirebaseApp.DefaultInstance == null)
                {
                    app = FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(fcmConfigJson)
                    });
                }
                else
                {
                    app = FirebaseApp.DefaultInstance;
                }

                firebaseMessaging = FirebaseMessaging.GetMessaging(app);

                _initialized = true;

                _logger.LogInformation("Firebase PushNotification client initialized");
            }
            finally
            {
                _initLock.Release();
            }
        }



        public async Task<string?> SendAuthnNotification(AuthnNotificationDTO authnNotification)
        {
            _logger.LogDebug("-->SendAuthnNotification");

            await EnsureInitializedAsync();

            string? result = null;

            // Validate input parameters
            if (null == authnNotification)
            {
                _logger.LogError("Invalid Input Parameter");
                return result;
            }

            _logger.LogDebug("AuthnNotification request:{0}",
                JsonConvert.SerializeObject(authnNotification));

            var logoUrl = _configuration["LogoUrl"] ?? string.Empty;

            var isArabic = authnNotification.Language?.ToLower() == "ar";

            _logger.LogInformation("AuthnNotification request:{0}",
                JsonConvert.SerializeObject(authnNotification));

            _logger.LogInformation($"isArabic: {isArabic}");

            var message = new Message()
            {
                Token = authnNotification.RegistrationToken,
                Data = new Dictionary<string, string>()
                {
                    ["AuthNToken"] = authnNotification.AuthnToken,
                    ["AuthNScheme"] = authnNotification.AuthnScheme,
                    ["RandomCodes"] = authnNotification.RandomCodes,
                    ["ApplicationName"] = authnNotification.ApplicationName,
                    ["DeviceName"] = authnNotification.DeviceName,
                    ["TimeStamp"] = authnNotification.TimeStamp,
                    ["EnglishTitle"] = "Authentication Request",
                    ["EnglishBody"] = "Please approve or deny",
                    ["ArabicTitle"] = "طلب المصادقة",
                    ["ArabicBody"] = "يرجى الموافقة أو الرفض",
                    ["Title"] = isArabic ? "طلب المصادقة" : "Authentication Request",
                    ["Body"] = isArabic ? "يرجى الموافقة أو الرفض" : "Please approve or deny",
                    ["urlImage"] = logoUrl
                },

                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                },

                Apns = new ApnsConfig()
                {
                    Aps = new Aps()
                    {
                        Sound = "default",

                        Alert = new ApsAlert()
                        {
                            Title = isArabic ? "طلب المصادقة" : "Authentication Request",
                            Body = isArabic ? "يرجى الموافقة أو الرفض" : "Please approve or deny",
                        },

                        MutableContent = true

                    },
                },
            };

            try
            {
                _logger.LogDebug("Sending Push Notification");

                if (firebaseMessaging == null)
                {
                    _logger.LogError("FirebaseMessaging not initialized");
                    return null;
                }

                result = await firebaseMessaging.SendAsync(message);
                if (null == result)
                {
                    _logger.LogError("Send Notification Failed");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SendAuthnNotification Failed: {0}",
                    ex.Message);
                return result;
            }

            _logger.LogInformation("Send Notification Response:{0}", result);
            _logger.LogDebug("<--SendAuthnNotification");
            return result;
        }

        public async Task<string?> SendNotification(PushNotificationRequest request)
        {
            _logger.LogDebug("-->SendNotification");

            await EnsureInitializedAsync();

            string? result = null;

            // Validate input parameters
            if (null == request)
            {
                _logger.LogError("Invalid Input Parameter");
                return result;
            }

            var logoUrl = _configuration["LogoUrl"] ?? string.Empty;

            var message = new Message()
            {
                Token = request.RegistrationToken,
                Data = new Dictionary<string, string?>()
                {
                    ["Title"] = request.Title,
                    ["Body"] = request.Body,
                    ["Context"] = request.Context,
                    ["ConsentData"] = request.Text,
                    ["urlImage"] = logoUrl
                },

                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                },

                Apns = new ApnsConfig()
                {
                    Aps = new Aps()
                    {
                        Sound = "default",

                        Alert = new ApsAlert()
                        {
                            Title = request.Title,
                            Body = request.Body,
                        },

                        MutableContent = true,
                    },
                },
            };

            try
            {
                _logger.LogDebug("Sending Push Notification");

                if (firebaseMessaging == null)
                {
                    _logger.LogError("FirebaseMessaging not initialized");
                    return null;
                }

                result = await firebaseMessaging.SendAsync(message);
                if (null == result)
                {
                    _logger.LogError("Send Notification Failed");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SendNotification Failed: {0}",
                    ex.Message);
                return result;
            }

            _logger.LogInformation("SendNotification Response:{0}", result);
            _logger.LogDebug("<--SendNotification");
            return result;
        }

        public async Task<string?> SendEConsentNotification(EConsentNotificationDTO eConsentNotification)
        {
            _logger.LogDebug("-->SendEConsentNotification");

            await EnsureInitializedAsync();

            string? result = null;

            // Validate input parameters
            if (null == eConsentNotification)
            {
                _logger.LogError("Invalid Input Parameter");
                return result;
            }

            _logger.LogDebug("eConsentNotification request:{0}",
                JsonConvert.SerializeObject(eConsentNotification));

            string DeselectScopesAndClaims = string.Empty;
            if (eConsentNotification.DeselectScopesAndClaims)
                DeselectScopesAndClaims = "true";
            else
                DeselectScopesAndClaims = "false";


            var consentScopes = JsonConvert.SerializeObject(
                eConsentNotification.ConsentScopes);

            var logoUrl = _configuration["LogoUrl"] ?? string.Empty;

            var message = new Message()
            {
                Token = eConsentNotification.RegistrationToken,
                //Token = "e8XxUkt9yEhhtoXXq9HXAs:APA91bG45vfIvxn3TvAsvat6zqe-P628oZrfxUiil6i7SaInIQ2kc8Xu-Nww0K3JZ0oP1rM5kjIepxRepfVjDByaBH_YcvM0SmPxPTSqMl3eegoMySEdbaZlFqNaKh3uS8lMjvQuoNRu",
                Data = new Dictionary<string, string>()
                {
                    ["AuthNToken"] = eConsentNotification.AuthnToken,
                    ["AuthNScheme"] = eConsentNotification.AuthnScheme,
                    ["ApplicationName"] = eConsentNotification.ApplicationName,
                    ["ConsentScopes"] = consentScopes,
                    ["DeselectScopesAndClaims"] = DeselectScopesAndClaims,
                    ["Title"] = "Consent Request",
                    ["Body"] = "Please approve or deny",
                    ["urlImage"] = logoUrl
                },

                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                },

                Apns = new ApnsConfig()
                {
                    Aps = new Aps()
                    {
                        Sound = "default",

                        Alert = new ApsAlert()
                        {
                            Title = "Consent Request",
                            Body = "Please approve or deny",
                        },

                        MutableContent = true,

                    },
                },
            };

            try
            {
                _logger.LogInformation("Sending Push Notification");

                if (firebaseMessaging == null)
                {
                    _logger.LogError("FirebaseMessaging not initialized");
                    return null;
                }

                result = await firebaseMessaging.SendAsync(message);
                if (null == result)
                {
                    _logger.LogError("Send Notification Failed");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SendEConsentNotification Failed: {0}",
                    ex.Message);
                return result;
            }

            _logger.LogInformation("Send Notification Response:{0}", result);
            _logger.LogDebug("<--SendEConsentNotification");
            return result;
        }
    }
}
