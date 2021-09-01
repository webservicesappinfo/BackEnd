using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NotificationService;
using NotificationService.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotificationServiceImp : Notification.NotificationBase
    {
        private readonly ILogger<NotificationServiceImp> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMobileMessagingService _messaging;
        private readonly INotificationRepoService _notificationRepoService;

        public NotificationServiceImp(ILogger<NotificationServiceImp> logger, ILoggerFactory loggerFactory,
            IMobileMessagingService messaging, INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _messaging = messaging;
            _notificationRepoService = notificationRepoService;
        }

        public override Task<FindLastGetMessageReply> FindLastGetMessage(FindLastGetMessageRequest request, ServerCallContext context)
        {
            return Task.FromResult(new FindLastGetMessageReply());
        }

        public override Task<SendNotificationReply> SendNotification(SendNotificationRequest request, ServerCallContext context)
        {
            var reply = new SendNotificationReply() { Status = false };
            var forUser = _notificationRepoService.GetUser(request.ForGuid);
            var fromUser = _notificationRepoService.GetUser(request.FromGuid);

            if (forUser == null || fromUser == null) return Task.FromResult(reply);

            _notificationRepoService.SetLastSendMessage(fromUser.Guid, $"{forUser.Guid}:{request.Msg}");
            _notificationRepoService.SetLastGetMessage(forUser.Guid, $"{fromUser.Guid}:{request.Msg}");

            _messaging.SendNotification(forUser.Token, "NewMsg", request.Msg);

            reply.Status = true;
            return Task.FromResult(reply);
        }
    }
}
