using AuthService.Protos;
using CompanyService.Protos;
using FluentAssertions;
using Grpc.Core;
using LocationService.Protos;
using MobileApiGetway;
using MobileApiGetway.Services;
using Globals.Tests;
using Moq;
using NotificationService.Protos;
using OfferService.Protos;
using OrderService.Protos;
using SkillService.Protos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Protos;
using Xunit;

namespace MobileApiGetway.Tests.Services
{
    public class MobileApiServiceTests
    {
        private readonly Mock<User.UserClient> _mockUserClient = new();
        private readonly Mock<Auth.AuthClient> _mockAuthClient = new();
        private readonly Mock<Company.CompanyClient> _mockCompanyClient = new();
        private readonly Mock<Notification.NotificationClient> _mockNotificationClient = new();
        private readonly Mock<Skill.SkillClient> _mockSkillClient = new();
        private readonly Mock<Offer.OfferClient> _mockOfferClient = new();
        private readonly Mock<Order.OrderClient> _mockOrderClient = new();
        private readonly Mock<LocationRepo.LocationRepoClient> _mockLocationClient = new();

        private readonly MobileApiService _service;
        private readonly TestServerCallContext _context;

        public MobileApiServiceTests()
        {
            _service = new MobileApiService(
                _mockUserClient.Object,
                _mockAuthClient.Object,
                _mockCompanyClient.Object,
                _mockNotificationClient.Object,
                _mockSkillClient.Object,
                _mockOfferClient.Object,
                _mockOrderClient.Object,
                _mockLocationClient.Object
            );
            _context = TestServerCallContext.Create();
        }

        // ==========================================
        // 1. GetMainDataForUser Aggregation Tests
        // ==========================================

        [Fact]
        public async Task GetMainDataForUser_WhenUserAndCompaniesAndOffersExist_AggregatesAllData()
        {
            // Arrange
            var userUid = "fb-user-123";
            var userReply = new GetUserReply { Name = "Jane Doe", UidFB = userUid };
            _mockUserClient
                .Setup(c => c.GetUser(It.Is<GetUserRequest>(r => r.UidFB == userUid), null, null, default))
                .Returns(userReply);

            var company = new CompanyReply { Guid = "comp-1", Name = "Beauty Salon" };
            var master = new MasterReply { UidFB = "mast-1", Name = "Master 1" };
            master.Offers.Add(new OfferApi { Guid = "off-1" });
            master.Offers.Add(new OfferApi { Guid = "off-2" });
            company.Masters.Add(master);

            var companiesReply = new GetCompaniesReply();
            companiesReply.Companies.Add(company);

            _mockCompanyClient
                .Setup(c => c.GetCompanies(It.Is<GetCompaniesRequest>(r => r.UserUIDFB == userUid && r.Type == "owner"), null, null, default))
                .Returns(companiesReply);

            var populatedOffers = new GetOffersReply();
            populatedOffers.Offers.Add(new OfferApi { Guid = "off-1", Name = "Haircut", Desc = "Short hair" });
            populatedOffers.Offers.Add(new OfferApi { Guid = "off-2", Name = "Coloring", Desc = "Full color" });

            _mockOfferClient
                .Setup(c => c.GetOffers(It.Is<GetOffersRequest>(r => r.Guids.Contains("off-1") && r.Guids.Contains("off-2")), null, null, default))
                .Returns(populatedOffers);

            // Act
            var request = new GetMainDataForUserRequest { UserUIDFB = userUid };
            var result = await _service.GetMainDataForUser(request, _context);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("Jane Doe");
            result.UserUIDFB.Should().Be(userUid);
            result.Companies.Should().HaveCount(1);
            result.Companies[0].Masters.Should().HaveCount(1);
            result.Companies[0].Masters[0].Offers.Should().HaveCount(2);
            result.Companies[0].Masters[0].Offers[0].Name.Should().Be("Haircut");
            result.Companies[0].Masters[0].Offers[1].Name.Should().Be("Coloring");
        }

        [Fact]
        public async Task GetMainDataForUser_WhenUserHasNoCompanies_ReturnsEmptyCompanies()
        {
            var userUid = "fb-user-empty";
            _mockUserClient
                .Setup(c => c.GetUser(It.IsAny<GetUserRequest>(), null, null, default))
                .Returns(new GetUserReply { Name = "Solo User", UidFB = userUid });

            _mockCompanyClient
                .Setup(c => c.GetCompanies(It.IsAny<GetCompaniesRequest>(), null, null, default))
                .Returns(new GetCompaniesReply());

            var result = await _service.GetMainDataForUser(new GetMainDataForUserRequest { UserUIDFB = userUid }, _context);

            result.UserName.Should().Be("Solo User");
            result.Companies.Should().BeEmpty();
            _mockOfferClient.Verify(c => c.GetOffers(It.IsAny<GetOffersRequest>(), null, null, default), Times.Never());
        }

        [Fact]
        public async Task GetMainDataForUser_WhenCompanyHasNoMasters_ReturnsCompanyWithoutCallingOfferClient()
        {
            var userUid = "fb-user-nomasters";
            _mockUserClient
                .Setup(c => c.GetUser(It.IsAny<GetUserRequest>(), null, null, default))
                .Returns(new GetUserReply { Name = "Owner", UidFB = userUid });

            var companiesReply = new GetCompaniesReply();
            companiesReply.Companies.Add(new CompanyReply { Guid = "comp-empty", Name = "Empty Salon" });

            _mockCompanyClient
                .Setup(c => c.GetCompanies(It.IsAny<GetCompaniesRequest>(), null, null, default))
                .Returns(companiesReply);

            var result = await _service.GetMainDataForUser(new GetMainDataForUserRequest { UserUIDFB = userUid }, _context);

            result.Companies.Should().HaveCount(1);
            result.Companies[0].Masters.Should().BeEmpty();
            _mockOfferClient.Verify(c => c.GetOffers(It.IsAny<GetOffersRequest>(), null, null, default), Times.Never());
        }

        [Fact]
        public async Task GetMainDataForUser_WhenUserNotFound_HandlesNullSafely()
        {
            _mockUserClient
                .Setup(c => c.GetUser(It.IsAny<GetUserRequest>(), null, null, default))
                .Returns((GetUserReply)null!);

            _mockCompanyClient
                .Setup(c => c.GetCompanies(It.IsAny<GetCompaniesRequest>(), null, null, default))
                .Returns(new GetCompaniesReply());

            var result = await _service.GetMainDataForUser(new GetMainDataForUserRequest { UserUIDFB = "unknown" }, _context);

            result.UserName.Should().Be("NotFound");
            result.UserUIDFB.Should().BeEmpty();
        }

        // ==========================================
        // 2. GetFitForCompanyUsers Tests
        // ==========================================

        [Fact]
        public async Task GetFitForCompanyUsers_WhenCompanyNotFound_ReturnsEmptyReply()
        {
            _mockCompanyClient
                .Setup(c => c.GetCompany(It.IsAny<GetCompanyRequest>(), null, null, default))
                .Returns((GetCompanyReply)null!);

            var result = await _service.GetFitForCompanyUsers(new GetFitForCompanyUsersRequest { CompanyGuid = "comp-404" }, _context);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetFitForCompanyUsers_WhenCompanyExists_ReturnsReply()
        {
            _mockCompanyClient
                .Setup(c => c.GetCompany(It.Is<GetCompanyRequest>(r => r.Guid == "comp-1"), null, null, default))
                .Returns(new GetCompanyReply { Company = new CompanyReply { Guid = "comp-1", Name = "Salon" } });

            var result = await _service.GetFitForCompanyUsers(new GetFitForCompanyUsersRequest { CompanyGuid = "comp-1" }, _context);

            result.Should().NotBeNull();
        }

        // ==========================================
        // 3. UserService Forwarding Tests
        // ==========================================

        [Fact]
        public async Task ApiAddUser_DelegatesToUserClient()
        {
            var request = new AddUserRequest { Name = "John", UidFB = "uid-1" };
            var expectedReply = new AddUserReply { Result = true };
            _mockUserClient.Setup(c => c.AddUser(request, null, null, default)).Returns(expectedReply);

            var reply = await _service.ApiAddUser(request, _context);

            reply.Should().BeSameAs(expectedReply);
            _mockUserClient.Verify(c => c.AddUser(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task ApiDelUser_DelegatesToUserClient()
        {
            var request = new DelUserRequest { UidFB = "uid-1" };
            var expectedReply = new DelUserReply();
            _mockUserClient.Setup(c => c.DelUser(request, null, null, default)).Returns(expectedReply);

            var reply = await _service.ApiDelUser(request, _context);

            reply.Should().BeSameAs(expectedReply);
            _mockUserClient.Verify(c => c.DelUser(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task ApiGetUser_DelegatesToUserClient()
        {
            var request = new GetUserRequest { UidFB = "uid-1" };
            var expectedReply = new GetUserReply { Name = "John" };
            _mockUserClient.Setup(c => c.GetUser(request, null, null, default)).Returns(expectedReply);

            var reply = await _service.ApiGetUser(request, _context);

            reply.Should().BeSameAs(expectedReply);
            _mockUserClient.Verify(c => c.GetUser(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task ApiGetUsers_DelegatesToUserClient()
        {
            var request = new GetUsersRequest();
            var expectedReply = new GetUsersReply();
            _mockUserClient.Setup(c => c.GetUsers(request, null, null, default)).Returns(expectedReply);

            var reply = await _service.ApiGetUsers(request, _context);

            reply.Should().BeSameAs(expectedReply);
            _mockUserClient.Verify(c => c.GetUsers(request, null, null, default), Times.Once());
        }

        // ==========================================
        // 4. AuthService Async Forwarding Tests
        // ==========================================

        [Fact]
        public async Task ApiRegister_DelegatesToAuthClient_PassingCancellationToken()
        {
            var cts = new CancellationTokenSource();
            var context = TestServerCallContext.Create(cts.Token);
            var request = new RegisterRequest { Email = "test@example.com", Password = "Pass" };
            var expected = new RegisterReply { IsSuccess = true };

            _mockAuthClient
                .Setup(c => c.RegisterAsync(request, null, null, cts.Token))
                .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(expected));

            var result = await _service.ApiRegister(request, context);

            result.Should().BeSameAs(expected);
            _mockAuthClient.Verify(c => c.RegisterAsync(request, null, null, cts.Token), Times.Once());
        }

        [Fact]
        public async Task ApiLogin_DelegatesToAuthClient_PassingCancellationToken()
        {
            var cts = new CancellationTokenSource();
            var context = TestServerCallContext.Create(cts.Token);
            var request = new LoginRequest { Login = "test@example.com", Password = "Pass" };
            var expected = new LoginReply { Token = "jwt-token", IsSuccess = true };

            _mockAuthClient
                .Setup(c => c.LoginAsync(request, null, null, cts.Token))
                .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(expected));

            var result = await _service.ApiLogin(request, context);

            result.Should().BeSameAs(expected);
            _mockAuthClient.Verify(c => c.LoginAsync(request, null, null, cts.Token), Times.Once());
        }

        [Fact]
        public async Task ApiGoogleLogin_DelegatesToAuthClient()
        {
            var request = new GoogleLoginRequest { IdToken = "google-id-token" };
            var expected = new GoogleLoginReply { Token = "jwt-token", IsSuccess = true };

            _mockAuthClient
                .Setup(c => c.GoogleLoginAsync(request, null, null, It.IsAny<CancellationToken>()))
                .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(expected));

            var result = await _service.ApiGoogleLogin(request, _context);

            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task ApiLogout_DelegatesToAuthClient()
        {
            var request = new LogoutRequest { RefreshToken = "refresh-token" };
            var expected = new LogoutReply { IsSuccess = true };

            _mockAuthClient
                .Setup(c => c.LogoutAsync(request, null, null, It.IsAny<CancellationToken>()))
                .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(expected));

            var result = await _service.ApiLogout(request, _context);

            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task ApiChangePassword_DelegatesToAuthClient()
        {
            var request = new ChangePasswordRequest { UserId = 1, CurrentPassword = "old", NewPassword = "new" };
            var expected = new ChangePasswordReply { IsSuccess = true };

            _mockAuthClient
                .Setup(c => c.ChangePasswordAsync(request, null, null, It.IsAny<CancellationToken>()))
                .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(expected));

            var result = await _service.ApiChangePassword(request, _context);

            result.Should().BeSameAs(expected);
        }

        // ==========================================
        // 5. CompanyService Forwarding Tests
        // ==========================================

        [Fact]
        public async Task AddCompany_DelegatesToCompanyClient()
        {
            var request = new AddCompanyRequest { Name = "Saloon" };
            var expected = new AddCompanyReply { Result = true };
            _mockCompanyClient.Setup(c => c.AddCompany(request, null, null, default)).Returns(expected);

            var reply = await _service.AddCompany(request, _context);

            reply.Should().BeSameAs(expected);
            _mockCompanyClient.Verify(c => c.AddCompany(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task DelCompany_DelegatesToCompanyClient()
        {
            var request = new DelCompanyRequest { Guid = "comp-1" };
            var expected = new DelCompanyReply { Result = true };
            _mockCompanyClient.Setup(c => c.DelCompany(request, null, null, default)).Returns(expected);

            var reply = await _service.DelCompany(request, _context);

            reply.Should().BeSameAs(expected);
            _mockCompanyClient.Verify(c => c.DelCompany(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task AddMaster_DelegatesToCompanyClient()
        {
            var request = new AddMasterRequest { Master = new MasterReply { Name = "Master 1" } };
            var expected = new AddMasterReply { Result = true };
            _mockCompanyClient.Setup(c => c.AddMaster(request, null, null, default)).Returns(expected);

            var reply = await _service.AddMaster(request, _context);

            reply.Should().BeSameAs(expected);
            _mockCompanyClient.Verify(c => c.AddMaster(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task DelMaster_DelegatesToCompanyClient()
        {
            var request = new DelMasterRequest { Master = new MasterReply { Name = "Master 1" } };
            var expected = new DelMasterReply { Result = true };
            _mockCompanyClient.Setup(c => c.DelMaster(request, null, null, default)).Returns(expected);

            var reply = await _service.DelMaster(request, _context);

            reply.Should().BeSameAs(expected);
            _mockCompanyClient.Verify(c => c.DelMaster(request, null, null, default), Times.Once());
        }

        // ==========================================
        // 6. Offer & Order Service Forwarding Tests
        // ==========================================

        [Fact]
        public async Task AddOffer_DelegatesToOfferClient()
        {
            var request = new AddOfferRequest { Offer = new OfferApi { Name = "Nails" } };
            var expected = new AddOfferReply { Result = true };
            _mockOfferClient.Setup(c => c.AddOffer(request, null, null, default)).Returns(expected);

            var reply = await _service.AddOffer(request, _context);

            reply.Should().BeSameAs(expected);
            _mockOfferClient.Verify(c => c.AddOffer(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task DelOffer_DelegatesToOfferClient()
        {
            var request = new DelOfferRequest { Offer = new OfferApi { Guid = "off-1" } };
            var expected = new DelOfferReply { Result = true };
            _mockOfferClient.Setup(c => c.DelOffer(request, null, null, default)).Returns(expected);

            var reply = await _service.DelOffer(request, _context);

            reply.Should().BeSameAs(expected);
            _mockOfferClient.Verify(c => c.DelOffer(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task AddOrder_DelegatesToOrderClient()
        {
            var request = new AddOrderRequest { Order = new OrderApi { Name = "Booking" } };
            var expected = new AddOrderReply { Result = true };
            _mockOrderClient.Setup(c => c.AddOrder(request, null, null, default)).Returns(expected);

            var reply = await _service.AddOrder(request, _context);

            reply.Should().BeSameAs(expected);
            _mockOrderClient.Verify(c => c.AddOrder(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task DelOrder_DelegatesToOrderClient()
        {
            var request = new DelOrderRequest { Order = new OrderApi { Guid = "ord-1" } };
            var expected = new DelOrderReply { Result = true };
            _mockOrderClient.Setup(c => c.DelOrder(request, null, null, default)).Returns(expected);

            var reply = await _service.DelOrder(request, _context);

            reply.Should().BeSameAs(expected);
            _mockOrderClient.Verify(c => c.DelOrder(request, null, null, default), Times.Once());
        }

        // ==========================================
        // 7. NotificationService Forwarding Tests
        // ==========================================

        [Fact]
        public async Task ApiFindLastMessage_DelegatesToNotificationClient()
        {
            var request = new FindLastGetMessageRequest { FromGuid = "u-1", ForGuid = "u-2" };
            var expected = new FindLastGetMessageReply { Msg = "Hello" };
            _mockNotificationClient.Setup(c => c.FindLastGetMessage(request, null, null, default)).Returns(expected);

            var reply = await _service.ApiFindLastMessage(request, _context);

            reply.Should().BeSameAs(expected);
            _mockNotificationClient.Verify(c => c.FindLastGetMessage(request, null, null, default), Times.Once());
        }

        [Fact]
        public async Task ApiSendMessage_DelegatesToNotificationClient()
        {
            var request = new SendNotificationRequest { ForGuid = "u-1", FromGuid = "u-2", Msg = "Hello" };
            var expected = new SendNotificationReply { Status = true };
            _mockNotificationClient.Setup(c => c.SendNotification(request, null, null, default)).Returns(expected);

            var reply = await _service.ApiSendMessage(request, _context);

            reply.Should().BeSameAs(expected);
            _mockNotificationClient.Verify(c => c.SendNotification(request, null, null, default), Times.Once());
        }

        // ==========================================
        // 8. LocationService Mapping Tests
        // ==========================================

        [Fact]
        public async Task ApiGetUserLocation_MapsRequestAndResponse_Correctly()
        {
            var request = new ApiGetUserLocationRequest { Guid = "u-loc-1" };
            var locationReply = new LocationService.Protos.GetUserLocationReply { ForGuid = "u-loc-1", Lat = "50.4501", Lng = "30.5234" };

            _mockLocationClient
                .Setup(c => c.GetUserLocation(It.Is<LocationService.Protos.GetUserLocationRequest>(r => r.Guid == "u-loc-1"), null, null, default))
                .Returns(locationReply);

            var reply = await _service.ApiGetUserLocation(request, _context);

            reply.Should().NotBeNull();
            reply.ForGuid.Should().Be("u-loc-1");
            reply.Lat.Should().Be("50.4501");
            reply.Lng.Should().Be("30.5234");
        }

        [Fact]
        public async Task ApiSetUserLocation_MapsRequestAndResponse_Correctly()
        {
            var request = new ApiSetUserLocationRequest { ForGuid = "u-loc-1", Lat = "50.4501", Lng = "30.5234" };
            var locationReply = new LocationService.Protos.SetUserLocationReply { IsSet = true };

            _mockLocationClient
                .Setup(c => c.SetUserLocation(
                    It.Is<LocationService.Protos.SetUserLocationRequest>(r => r.ForGuid == "u-loc-1" && r.Lat == "50.4501" && r.Lng == "30.5234"),
                    null, null, default))
                .Returns(locationReply);

            var reply = await _service.ApiSetUserLocation(request, _context);

            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();
        }
    }
}
