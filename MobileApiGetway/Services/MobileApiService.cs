using CompanyService.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using OfferService.Protos;
using OrderService.Protos;
using SkillService.Protos;
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
        private readonly Company.CompanyClient _companyClient;
        private readonly Skill.SkillClient _skillClient;
        private readonly Offer.OfferClient _offerClient;
        private readonly Order.OrderClient _orderClient;

        private readonly Notification.NotificationClient _notificationClient;
        //private readonly UserRepo.UserRepoClient _userRepoClient;
        private readonly LocationRepo.LocationRepoClient _locationClient;

        public MobileApiService
            (
            User.UserClient userClient,
            Company.CompanyClient companyClient,
            Notification.NotificationClient notificationClient,
            Skill.SkillClient skillClient,
            Offer.OfferClient offerClient,
            Order.OrderClient orderClient,
            /*UserRepo.UserRepoClient userRepoClient,*/
            LocationRepo.LocationRepoClient locationClient
            )
        {
            _userClient = userClient;
            _companyClient = companyClient;
            _skillClient = skillClient;
            _offerClient = offerClient;
            _orderClient = orderClient;

            _notificationClient = notificationClient;
            //_userRepoClient = userRepoClient;
            _locationClient = locationClient;
        }

        #region UserRepoService
        public override Task<AddUserReply> ApiAddUser(AddUserRequest request, ServerCallContext context)
        {
            /*var reply = _userRepoClient.AddUser(new AddUserRequest()
            {
                Name = request.Name,
                Guid = request.Guid,
                Token = request.Token
            });*/

            var reply = _userClient.AddUser(request);
            return Task.FromResult(new AddUserReply() { Result = reply.Result });
        }
        public override Task<DelUserReply> ApiDelUser(DelUserRequest request, ServerCallContext context)
        {
            var reply = _userClient.DelUser(request);
            return Task.FromResult(new DelUserReply() { Result = reply.Result });
        }

        public override Task<GetUserReply> ApiGetUser(GetUserRequest request, ServerCallContext context)
        {
            /*var reply = _userRepoClient.GetUser(new GetUserRequest() { Guid = request.Guid });
            return Task.FromResult(new ApiGetUserReply()
            {
                Guid = reply.Guid,
                Name = reply.Name
            });*/

            var reply = _userClient.GetUser(request);
            return Task.FromResult(new GetUserReply()
            {
                UidFB = reply.UidFB,
                Name = reply.Name
            });
        }

        public override Task<GetUsersReply> ApiGetUsers(GetUsersRequest request, ServerCallContext context)
        {
            /*var reply = _userRepoClient.GetUsers(new GetUsersRequest());
            var apiReply = new ApiGetUsersReply();
            foreach (var n in reply.Names)
                apiReply.Names.Add(n);
            return Task.FromResult(apiReply);*/

            var reply = _userClient.GetUsers(request);
            var apiReply = new GetUsersReply();
            foreach (var n in reply.Names)
                apiReply.Names.Add(n);
            return Task.FromResult(apiReply);
        }
        #endregion

        #region CompanyService
        public override Task<AddCompanyReply> ApiAddCompany(AddCompanyRequest request, ServerCallContext context)
        {
            var reply = _companyClient.AddCompany(request);
            return Task.FromResult(new AddCompanyReply() { Result = reply.Result });
        }

        public override Task<JoinToCompanyReply> ApiJoinToCompany(JoinToCompanyRequest request, ServerCallContext context)
        {
            var reply = _companyClient.JoinToCompany(request);
            return Task.FromResult(new JoinToCompanyReply() { Result = reply.Result });
        }

        public override Task<GetCompanyReply> ApiGetCompany(GetCompanyRequest request, ServerCallContext context)
        {
            var reply = _companyClient.GetCompany(request);
            return Task.FromResult(new GetCompanyReply()
            {
                Guid = reply.Guid,
                Name = reply.Name
            });
        }

        public override Task<GetCompaniesReply> ApiGetCompanies(GetCompaniesRequest request, ServerCallContext context)
        {
            var reply = _companyClient.GetCompanies(request);
            return Task.FromResult(reply);
        }

        public override Task<DelCompanyReply> ApiDelCompany(DelCompanyRequest request, ServerCallContext context)
        {
            var reply = _companyClient.DelCompany(request);
            return Task.FromResult(reply);
        }
        #endregion

        #region SkillService
        public override Task<AddSkillReply> ApiAddSkill(AddSkillRequest request, ServerCallContext context)
        {
            var reply = _skillClient.AddSkill(request);
            return Task.FromResult(reply);
        }
        public override Task<GetSkillsReply> ApiGetSkills(GetSkillsRequest request, ServerCallContext context)
        {
            var reply = _skillClient.GetSkills(request);
            return Task.FromResult(reply);
        }
        public override Task<DelSkillReply> ApiDelSkill(DelSkillRequest request, ServerCallContext context)
        {
            var reply = _skillClient.DelSkill(request);
            return Task.FromResult(reply);
        }
        #endregion

        #region OfferService
        public override Task<AddOfferReply> ApiAddOffer(AddOfferRequest request, ServerCallContext context)
            => Task.FromResult(_offerClient.AddOffer(request));

        public override Task<GetOffersReply> ApiGetOffersByMaster(GetOffersByMasterRequest request, ServerCallContext context)
            => Task.FromResult(_offerClient.GetOffersByMaster(request));

        public override Task<GetOffersReply> ApiGetOffersBySkill(GetOffersBySkillRequest request, ServerCallContext context)
            => Task.FromResult(_offerClient.GetOffersBySkill(request));

        public override Task<DelOfferReply> ApiDelOffer(DelOfferRequest request, ServerCallContext context)
            => Task.FromResult(_offerClient.DelOffer(request));
        #endregion

        #region OrderService
        public override Task<AddOrderReply> ApiAddOrder(AddOrderRequest request, ServerCallContext context)
        {
            var reply = _orderClient.AddOrder(request);
            return Task.FromResult(reply);
        }
        public override Task<GetOrdersReply> ApiGetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            var reply = _orderClient.GetOrders(request);
            return Task.FromResult(reply);
        }
        public override Task<DelOrderReply> ApiDelOrder(DelOrderRequest request, ServerCallContext context)
        {
            var reply = _orderClient.DelOrder(request);
            return Task.FromResult(reply);
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
            return Task.FromResult(new ApiSetUserLocationReply() { Result = reply.IsSet });
        }
        #endregion
    }
}
