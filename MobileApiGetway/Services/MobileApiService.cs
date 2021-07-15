using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGetway.Services
{
    public class MobileApiService: MobileApi.MobileApiBase
    {
        public override Task<ApiAddUserReply> ApiAddUser(ApiAddUserRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://userreposervice", new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure });
            var client = new UserRepo.UserRepoClient(channel);
            var reply = client.AddUser(new AddUserRequest() 
            { 
                Name = request.Name, 
                Guid = request.Guid, 
                Token = request.Token
            });

            return Task.FromResult(new ApiAddUserReply() {IsAdded = reply .IsAdded});
        }

        public override Task<ApiGetUserReply> ApiGetUser(ApiGetUserRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://userreposervice", new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure });
            var client = new UserRepo.UserRepoClient(channel);
            var reply = client.GetUser(new GetUserRequest() { Guid = request.Guid });

            return Task.FromResult(new ApiGetUserReply()
            {
                Guid = reply.Guid,
                Name = reply.Name
            });
        }

        public override Task<ApiGetUsersReply> ApiGetUsers(ApiGetUsersRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://userreposervice", new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure });
            var client = new UserRepo.UserRepoClient(channel);
            var reply = client.GetUsers(new GetUsersRequest());

            var apiReply = new ApiGetUsersReply();
            foreach (var n in reply.Names)
                apiReply.Names.Add(n);

            return Task.FromResult(apiReply);
        }

        public override Task<ApiGetLastMessagesReply> ApiGetLastMessage(ApiGetLastMessageRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://userreposervice", new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure });
            var client = new UserRepo.UserRepoClient(channel);
            var reply = client.GetLastMessage(new GetLastMessageRequest() { Guid = request.Guid });

            return Task.FromResult(new ApiGetLastMessagesReply() 
            { 
                ForGuid = reply.ForGuid, 
                Msg = reply.Msg
            });
        }

        public override Task<ApiSendMessageReply> ApiSendMessage(ApiSendMessageRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://notificationservice", new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure });
            var client = new Notification.NotificationClient(channel);
            var reply = client.SendNotification(new SendNotificationRequest() 
            { 
                ForGuid = request.ForGuid, 
                FromGuid = request.FromGuid, 
                Msg = request.Msg 
            });

            return Task.FromResult(new ApiSendMessageReply() { Status = reply.Status });
        }
    }
}
