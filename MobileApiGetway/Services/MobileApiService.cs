﻿using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserService.Protos;

namespace MobileApiGetway.Services
{
    public class MobileApiService : MobileApi.MobileApiBase
    {
        private readonly User.UserClient _userClient;

        private readonly Notification.NotificationClient _notificationClient;
        //private readonly UserRepo.UserRepoClient _userRepoClient;
        private readonly LocationRepo.LocationRepoClient _locationClient;

        public MobileApiService(User.UserClient userClient, Notification.NotificationClient notificationClient, /*UserRepo.UserRepoClient userRepoClient,*/ LocationRepo.LocationRepoClient locationClient)
        {
            _userClient = userClient;

            _notificationClient = notificationClient;
            //_userRepoClient = userRepoClient;
            _locationClient = locationClient;
        }

        #region UserRepoService
        public override Task<ApiAddUserReply> ApiAddUser(ApiAddUserRequest request, ServerCallContext context)
        {
            /*var reply = _userRepoClient.AddUser(new AddUserRequest()
            {
                Name = request.Name,
                Guid = request.Guid,
                Token = request.Token
            });*/

            var reply = _userClient.AddUser(new AddUserRequest()
            {
                Name = request.Name,
                Guid = request.Guid,
                Token = request.Token
            });
            return Task.FromResult(new ApiAddUserReply() { Result = reply.Result});
        }

        public override Task<ApiGetUserReply> ApiGetUser(ApiGetUserRequest request, ServerCallContext context)
        {
            /*var reply = _userRepoClient.GetUser(new GetUserRequest() { Guid = request.Guid });
            return Task.FromResult(new ApiGetUserReply()
            {
                Guid = reply.Guid,
                Name = reply.Name
            });*/

            var reply = _userClient.GetUser(new GetUserRequest() { Guid = request.Guid });
            return Task.FromResult(new ApiGetUserReply()
            {
                Guid = reply.Guid,
                Name = reply.Name
            });
        }

        public override Task<ApiGetUsersReply> ApiGetUsers(ApiGetUsersRequest request, ServerCallContext context)
        {
            /*var reply = _userRepoClient.GetUsers(new GetUsersRequest());
            var apiReply = new ApiGetUsersReply();
            foreach (var n in reply.Names)
                apiReply.Names.Add(n);
            return Task.FromResult(apiReply);*/

            var reply = _userClient.GetUsers(new GetUsersRequest());
            var apiReply = new ApiGetUsersReply();
            foreach (var n in reply.Names)
                apiReply.Names.Add(n);
            return Task.FromResult(apiReply);
        }
        #endregion

        #region NotificationService
        public override Task<ApiFindLastMessagesReply> ApiFindLastMessage(ApiFindLastMessageRequest request, ServerCallContext context)
        {
            var reply = _notificationClient.FindLastGetMessage(new FindLastGetMessageRequest() { ForGuid = request.ForGuid, FromGuid = request.FromGuid });
            return Task.FromResult(new ApiFindLastMessagesReply() { Msg = reply.Msg });
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
        #endregion

        #region LocationService
        public override Task<ApiGetUserLocationReply> ApiGetUserLocation(ApiGetUserLocationRequest request, ServerCallContext context)
        {
            var reply = _locationClient.GetUserLocation(new GetUserLocationRequest() { Guid = request.Guid });
            return Task.FromResult(new ApiGetUserLocationReply() 
            { 
                ForGuid = reply.ForGuid, 
                Lat = reply.Lat, 
                Lng = reply.Lng
            });
        }

        public override Task<ApiSetUserLocationReply> ApiSetUserLocation(ApiSetUserLocationRequest request, ServerCallContext context)
        {
            var reply = _locationClient.SetUserLocation(new SetUserLocationRequest() 
            { 
                ForGuid = request.ForGuid, 
                Lat = request.Lat, 
                Lng = request.Lng
            });
            return Task.FromResult(new ApiSetUserLocationReply() { Result = reply.IsSet});
        }
        #endregion
    }
}
