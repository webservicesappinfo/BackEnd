using CompanyService.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using NotificationService.Protos;
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

        #region MobileApi
        public override Task<GetMainDataForUserReply> GetMainDataForUser(GetMainDataForUserRequest request, ServerCallContext context)
        {
            var reply = new GetMainDataForUserReply();
            var fitUser = _userClient.GetUser(new GetUserRequest() { UidFB = request.UserUIDFB });
            reply.UserName = fitUser?.Name ?? "NotFound";
            reply.UserUIDFB = fitUser.UidFB;
            var response = _companyClient.GetCompanies(new GetCompaniesRequest() { UserUIDFB = request.UserUIDFB, Type = "owner" });
            foreach(var company in response.Companies)
                foreach(var master in company.Masters)
                {
                    var offerRequest = new GetOffersRequest();
                    offerRequest.Guids.AddRange(master.Offers.Select(x => x.Guid));
                    var offerReply = _offerClient.GetOffers(offerRequest);
                    master.Offers.Clear();
                    master.Offers.AddRange(offerReply.Offers);
                }
            reply.Companies.AddRange(response.Companies);
            return Task.FromResult(reply);
        }

        public override Task<GetFitForCompanyUsersReply> GetFitForCompanyUsers(GetFitForCompanyUsersRequest request, ServerCallContext context)
        {
            var reply = new GetFitForCompanyUsersReply();
            var company = _companyClient.GetCompany(new GetCompanyRequest() { Guid = request.CompanyGuid });
            if (company == null) return Task.FromResult(reply);
            /*if (request.IsConsistIn)
            {
                for (var i = 0; i < company.MasterGuids.Count; i++)
                {
                    reply.Guids.Add(company.MasterGuids[i]);
                    reply.Names.Add(company.MasterNames[i]);
                }
            }
            else
            {
                var users = _userClient.GetUsers(new GetUsersRequest());
                for (var i = 0; i < users.Uids.Count; i++)
                {
                    if (company.MasterGuids.Any(x => x == users.Uids[i])) continue;
                    reply.Guids.Add(users.Uids[i]);
                    reply.Names.Add(users.Names[i]);
                }
            }*/
            return Task.FromResult(reply);
        }

        #endregion

        #region UserService
        public override Task<AddUserReply> ApiAddUser(AddUserRequest request, ServerCallContext context)  => Task.FromResult(_userClient.AddUser(request));
        public override Task<DelUserReply> ApiDelUser(DelUserRequest request, ServerCallContext context) => Task.FromResult(_userClient.DelUser(request));
        public override Task<GetUserReply> ApiGetUser(GetUserRequest request, ServerCallContext context) => Task.FromResult(_userClient.GetUser(request));
        public override Task<GetUsersReply> ApiGetUsers(GetUsersRequest request, ServerCallContext context) => Task.FromResult(_userClient.GetUsers(request));
        #endregion

        #region CompanyService
        public override Task<AddCompanyReply> AddCompany(AddCompanyRequest request, ServerCallContext context) => Task.FromResult(_companyClient.AddCompany(request));
        public override Task<DelCompanyReply> DelCompany(DelCompanyRequest request, ServerCallContext context) => Task.FromResult(_companyClient.DelCompany(request));
        public override Task<AddMasterReply> AddMaster(AddMasterRequest request, ServerCallContext context) => Task.FromResult(_companyClient.AddMaster(request));
        public override Task<DelMasterReply> DelMaster(DelMasterRequest request, ServerCallContext context) => Task.FromResult(_companyClient.DelMaster(request));
        #endregion

        #region SkillService
        /*public override Task<AddSkillReply> ApiAddSkill(AddSkillRequest request, ServerCallContext context)
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
        }*/
        #endregion

        #region OfferService
        public override Task<AddOfferReply> AddOffer(AddOfferRequest request, ServerCallContext context) => Task.FromResult(_offerClient.AddOffer(request));
        public override Task<DelOfferReply> DelOffer(DelOfferRequest request, ServerCallContext context) => Task.FromResult(_offerClient.DelOffer(request));
        #endregion

        #region OrderService
        public override Task<AddOrderReply> AddOrder(AddOrderRequest request, ServerCallContext context) => Task.FromResult(_orderClient.AddOrder(request));
        public override Task<DelOrderReply> DelOrder(DelOrderRequest request, ServerCallContext context) => Task.FromResult(_orderClient.DelOrder(request));
        #endregion

        #region NotificationService
        public override Task<FindLastGetMessageReply> ApiFindLastMessage(FindLastGetMessageRequest request, ServerCallContext context)
        {
            var reply = _notificationClient.FindLastGetMessage(request);
            return Task.FromResult(reply);
        }
        public override Task<SendNotificationReply> ApiSendMessage(SendNotificationRequest request, ServerCallContext context)
        {
            var reply = _notificationClient.SendNotification(request);
            return Task.FromResult(reply);
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
