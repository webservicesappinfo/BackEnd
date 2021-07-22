using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGetway.Services
{
    public class MobileApiService : MobileApi.MobileApiBase
    {
        private readonly Notification.NotificationClient _notificationClient;
        private readonly UserRepo.UserRepoClient _userRepoClient;

        public MobileApiService(Notification.NotificationClient notificationClient, UserRepo.UserRepoClient userRepoClient)
        {
            _notificationClient = notificationClient;
            _userRepoClient = userRepoClient;
        }

        public override Task<ApiAddUserReply> ApiAddUser(ApiAddUserRequest request, ServerCallContext context)
        {
            var reply = _userRepoClient.AddUser(new AddUserRequest()
            {
                Name = request.Name,
                Guid = request.Guid,
                Token = request.Token
            });
            return Task.FromResult(new ApiAddUserReply() { IsAdded = reply.IsAdded });
        }

        public override Task<ApiGetUserReply> ApiGetUser(ApiGetUserRequest request, ServerCallContext context)
        {
            var reply = _userRepoClient.GetUser(new GetUserRequest() { Guid = request.Guid });
            return Task.FromResult(new ApiGetUserReply()
            {
                Guid = reply.Guid,
                Name = reply.Name
            });
        }

        public override Task<ApiGetUsersReply> ApiGetUsers(ApiGetUsersRequest request, ServerCallContext context)
        {
            var reply = _userRepoClient.GetUsers(new GetUsersRequest());
            var apiReply = new ApiGetUsersReply();
            foreach (var n in reply.Names)
                apiReply.Names.Add(n);
            return Task.FromResult(apiReply);
        }

        public override Task<ApiGetLastMessagesReply> ApiGetLastMessage(ApiGetLastMessageRequest request, ServerCallContext context)
        {
            var reply = _userRepoClient.GetLastMessage(new GetLastMessageRequest() { Guid = request.Guid });
            return Task.FromResult(new ApiGetLastMessagesReply()
            {
                ForGuid = reply.ForGuid,
                Msg = reply.Msg
            });
        }

        public override Task<ApiSendMessageReply> ApiSendMessage(ApiSendMessageRequest request, ServerCallContext context)
        {
            var reply = _notificationClient.SendNotification(new SendNotificationRequest()
            {
                ForGuid = request.ForGuid,
                FromGuid = request.FromGuid,
                Msg = request.Msg
            });
            return Task.FromResult(new ApiSendMessageReply() { Status = reply.Status });
        }
    }
}
