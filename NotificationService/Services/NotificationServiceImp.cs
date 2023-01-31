using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
using NotificationService;
using NotificationService.Abstractions;
using NotificationService.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotificationServiceImp : Notification.NotificationBase
    {
        private readonly ILogger<NotificationServiceImp> _logger;
        private readonly INotificationRepoService _notificationRepoService;

        public NotificationServiceImp(ILogger<NotificationServiceImp> logger,INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _notificationRepoService = notificationRepoService;
        }

        public override Task<FindLastGetMessageReply> FindLastGetMessage(FindLastGetMessageRequest request, ServerCallContext context)
        {
            return Task.FromResult(new FindLastGetMessageReply());
        }

        public override Task<SendNotificationReply> SendNotification(SendNotificationRequest request, ServerCallContext context)
        {
            var reply = new SendNotificationReply();
            reply.Status = _notificationRepoService.SendNotification(new Guid(request.FromGuid), new Guid(request.ForGuid), request.Msg);
            return Task.FromResult(reply);
        }
    }
}
