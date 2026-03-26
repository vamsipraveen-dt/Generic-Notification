using GenericNotification.Core.Domain.Models;
using GenericNotification.Core.Domain.Repositories;
using GenericNotification.Core.Domain.Services;
using GenericNotification.Core.Domain.Services.Communication;
using GenericNotification.Core.Domain.Services.Communication.Notification;
using GenericNotification.Core.DTO;
using GenericNotification.Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GenericNotification.Core.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationClient _pushNotificationClient;
        private readonly IConfiguration configuration;



        public PushNotificationService(
            ILogger<PushNotificationService> logger,
            IUnitOfWork unitOfWork,
            IPushNotificationClient pushNotificationClient,
            IConfiguration Configuration

            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _pushNotificationClient = pushNotificationClient;
            configuration = Configuration;

        }

        public async Task<ServiceResult> SendNotification(PushNotificationDTO request)
        {
            ServiceResult response;

            // Validate input
            if (
                (null == request) ||
                (string.IsNullOrEmpty(request.Suid)) ||
                (string.IsNullOrEmpty(request.Title)) ||
                (string.IsNullOrEmpty(request.Body))
                )
            {
                _logger.LogError("Invalid Argument");

                response = new ServiceResult("Invalid Argument");
                return response;
            }

            var raSubscriber = new Subscriber();
            try
            {
                raSubscriber = await _unitOfWork.Subscriber.GetSubscriberInfoBySUID(
                    request.Suid);
                if (null == raSubscriber)
                {
                    _logger.LogError("Subscriber details not found");

                    response = new ServiceResult("Subscriber details not found");

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = new ServiceResult($"Database Exception :: {ex.Message}");
                return response;
            }
            var context = string.IsNullOrEmpty(request.Context) ? configuration["NotificationContext"] : request.Context;

            if (raSubscriber.SubscriberFcmToken?.FcmToken == null)
            {
                _logger.LogError("Subscriber FCM token not found");

                return new ServiceResult("Subscriber device is not registered");
            }
            // Send notification to mobile
            var notificationRequest = new PushNotificationRequest()
            {
                Title = request.Title,
                Body = request.Body,
                Context = context,
                RegistrationToken = raSubscriber.SubscriberFcmToken.FcmToken,
            };
            var result = await _pushNotificationClient.SendNotification(
                notificationRequest);
            if (null == result)
            {
                _logger.LogError("SendNotification failed");
                response = new ServiceResult("Failed to Send Notification");
                return response;
            }

            response = new ServiceResult(true, "Push Notification Sent Successfully", true);
            return response;
        }

        public async Task<ServiceResult> SendAuthnNotification(AuthnNotificationDTO request)
        {
            ServiceResult response;

            // Validate input
            if (
                (null == request) ||
                (string.IsNullOrEmpty(request.AuthnToken)) ||
                (string.IsNullOrEmpty(request.AuthnScheme)) ||
                (string.IsNullOrEmpty(request.RegistrationToken)) ||
                (string.IsNullOrEmpty(request.ApplicationName)) ||
                (string.IsNullOrEmpty(request.RandomCodes))
                )
            {
                _logger.LogError("Invalid Argument");

                response = new ServiceResult("Invalid Argument");
                return response;
            }

            var result = await _pushNotificationClient.SendAuthnNotification(request);
            if (null == result)
            {
                _logger.LogError("SendAuthnNotification failed");
                response = new ServiceResult("Failed to Send Notification");
                return response;
            }

            response = new ServiceResult(true, "Push Notification Sent Successfully", true);
            return response;
        }

        public async Task<ServiceResult> SendEConsentNotification(EConsentNotificationDTO request)
        {
            ServiceResult response;

            // Validate input
            if (
                (null == request) ||
                (string.IsNullOrEmpty(request.AuthnToken)) ||
                (string.IsNullOrEmpty(request.AuthnScheme)) ||
                (string.IsNullOrEmpty(request.RegistrationToken)) ||
                (string.IsNullOrEmpty(request.ApplicationName)) ||
                (request.ConsentScopes !=null )
                )
            {
                _logger.LogError("Invalid Argument");

                response = new ServiceResult("Invalid Argument");
                return response;
            }

            var result = await _pushNotificationClient.SendEConsentNotification(request);
            if (null == result)
            {
                _logger.LogError("SendAuthnNotification failed");
                response = new ServiceResult("Failed to Send Notification");
                return response;
            }

            response = new ServiceResult(true, "Push Notification Sent Successfully", true);
            return response;
        }

        public async Task<ServiceResult> SendNotificationDelegationRequest(DelegationPushNotificationDTO request)
        {
            ServiceResult response;

            // Validate input
            if (request == null ||
      request.DelegateeList == null ||
      request.DelegateeList.Count == 0 ||
      string.IsNullOrWhiteSpace(request.Title) ||
      string.IsNullOrWhiteSpace(request.Body))
            {
                _logger.LogError("Invalid Argument");
                return new ServiceResult("Invalid Argument");
            }

            List<Subscriber> raSubscriber;
            try
            {
                raSubscriber = await _unitOfWork.Subscriber
            .GetSubscriberInfoBySUIDList(request.DelegateeList)
            ?? new List<Subscriber>();

              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                response = new ServiceResult($"Database Exception :: {ex.Message}");

                return response;
            }
            if (raSubscriber == null || raSubscriber.Count == 0)
            {
                _logger.LogError("Subscriber details not found");
                return new ServiceResult("Subscriber details not found");
            }
            foreach (var subscriber in raSubscriber)
            {
                var context = "SIGNATURE_DELEGATION";
                if (request.IsDelegator)
                {
                    context = "SIGNATURE_DELEGATION_DELEGATOR";
                    if (request.IsIdle)
                    {
                        context = "SIGNATURE_DELEGATEE_ACTION";
                    }
                }
                if (subscriber.SubscriberFcmToken?.FcmToken == null)
                {
                    _logger.LogError("Subscriber FCM token not found");

                    return new ServiceResult("Subscriber device is not registered");
                }
                // Send notification to mobile
                var notificationRequest = new PushNotificationRequest()
                {
                    Title = request.Title,
                    Body = request.Body,
                    Context = context,
                    Text = request.ConsentData,
                    RegistrationToken = subscriber.SubscriberFcmToken.FcmToken,
                };

                var result = await _pushNotificationClient.SendNotification(
                    notificationRequest);
                if (null == result)
                {
                    _logger.LogError("SendNotification failed");

                    response = new ServiceResult("Failed to Send Notification");

                    return response;
                }
            }

            response = new ServiceResult(true, "Push Notification Sent Successfully", true);

            return response;
        }
    }
}
