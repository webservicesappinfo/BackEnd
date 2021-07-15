using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotificationServiceImp : Notification.NotificationBase
    {
        private readonly ILogger<NotificationServiceImp> _logger;
        private ILoggerFactory _loggerFactory;
        public NotificationServiceImp(ILogger<NotificationServiceImp> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public override Task<SendNotificationReply> SendNotification(SendNotificationRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://userreposervice", new GrpcChannelOptions()
            {
                Credentials = ChannelCredentials.Insecure,
                LoggerFactory = _loggerFactory
            });
            var client = new UserRepo.UserRepoClient(channel);
            var forUser = client.GetUser(new GetUserRequest() { Guid = request.ForGuid });
            var fromUser = client.GetUser(new GetUserRequest() { Guid = request.FromGuid });
            var reply = new SendNotificationReply() { Status = false };
            return Task.FromResult(reply);
        }
    }
}
