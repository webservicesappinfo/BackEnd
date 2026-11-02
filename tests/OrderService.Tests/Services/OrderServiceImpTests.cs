using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using FluentAssertions;
using Globals.Tests;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
using Moq;
using OrderService.Abstractions;
using OrderService.Models;
using OrderService.Services;
using Xunit;
using Order = OrderService.Models.Order;

namespace OrderService.Tests.Services
{
    public class OrderServiceImpTests
    {
        private readonly Mock<ILogger<OrderServiceImp>> _mockLogger;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<IOrderRepoService> _mockRepo;
        private readonly TestServerCallContext _callContext;
        private readonly OrderServiceImp _service;

        public OrderServiceImpTests()
        {
            _mockLogger = new Mock<ILogger<OrderServiceImp>>();
            _mockEventBus = new Mock<IEventBus>();
            _mockRepo = new Mock<IOrderRepoService>();
            _callContext = new TestServerCallContext();

            _service = new OrderServiceImp(
                _mockLogger.Object,
                _mockEventBus.Object,
                _mockRepo.Object);
        }

        #region AddOrder Tests

        [Fact]
        public async Task AddOrder_WhenValid_PersistsEntityAndPublishesAddOrderEvent()
        {
            // Arrange
            var expectedOrderGuid = Guid.NewGuid();
            var offerGuid = Guid.NewGuid();
            var userGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var skillGuid = Guid.NewGuid();

            var request = new AddOrderRequest
            {
                Order = new OrderApi
                {
                    Name = "Haircut Appointment",
                    OfferGuid = offerGuid.ToString(),
                    UserGuid = userGuid.ToString(),
                    UserName = "Jane Doe",
                    MasterGuid = masterGuid.ToString(),
                    MasterName = "Alex Master",
                    SkillGuid = skillGuid.ToString(),
                    SkillName = "Haircut"
                }
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<Order>()))
                .Callback<Order>(o => o.Guid = expectedOrderGuid)
                .Returns(true);

            // Act
            var reply = await _service.AddOrder(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.AddEntity(It.Is<Order>(o =>
                o.Name == "Haircut Appointment" &&
                o.OfferGuid == offerGuid &&
                o.UserGuid == userGuid &&
                o.UserName == "Jane Doe" &&
                o.MasterGuid == masterGuid &&
                o.MasterName == "Alex Master" &&
                o.SkillGuid == skillGuid &&
                o.SkillName == "Haircut")), Times.Once);

            _mockEventBus.Verify(b => b.Publish(It.Is<AddOrderEvent>(e =>
                e.Name == "Haircut Appointment" &&
                e.Guid == expectedOrderGuid &&
                e.OfferGuid == offerGuid &&
                e.MasterGuid == masterGuid &&
                e.ClientGuid == userGuid &&
                e.ClientName == "Jane Doe")), Times.Once);
        }

        [Fact]
        public async Task AddOrder_WhenOfferGuidAndUserGuidAreEmpty_DefaultsToGuidEmpty()
        {
            // Arrange
            var masterGuid = Guid.NewGuid();
            var skillGuid = Guid.NewGuid();

            var request = new AddOrderRequest
            {
                Order = new OrderApi
                {
                    Name = "Order without offer/user",
                    OfferGuid = string.Empty,
                    UserGuid = string.Empty,
                    UserName = string.Empty,
                    MasterGuid = masterGuid.ToString(),
                    MasterName = "Master",
                    SkillGuid = skillGuid.ToString(),
                    SkillName = "Skill"
                }
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<Order>())).Returns(true);

            // Act
            var reply = await _service.AddOrder(request, _callContext);

            // Assert
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.AddEntity(It.Is<Order>(o =>
                o.OfferGuid == Guid.Empty &&
                o.UserGuid == Guid.Empty &&
                o.UserName == string.Empty)), Times.Once);

            _mockEventBus.Verify(b => b.Publish(It.Is<AddOrderEvent>(e =>
                e.OfferGuid == Guid.Empty &&
                e.ClientGuid == Guid.Empty)), Times.Once);
        }

        [Fact]
        public async Task AddOrder_WhenRepositoryFails_DoesNotPublishEventAndReturnsFalse()
        {
            // Arrange
            var request = new AddOrderRequest
            {
                Order = new OrderApi
                {
                    Name = "Order",
                    MasterGuid = Guid.NewGuid().ToString(),
                    MasterName = "Master",
                    SkillGuid = Guid.NewGuid().ToString(),
                    SkillName = "Skill"
                }
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<Order>())).Returns(false);

            // Act
            var reply = await _service.AddOrder(request, _callContext);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddOrderEvent>()), Times.Never);
        }

        [Fact]
        public async Task AddOrder_WhenMasterGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new AddOrderRequest
            {
                Order = new OrderApi
                {
                    Name = "Order",
                    MasterGuid = "invalid-guid",
                    SkillGuid = Guid.NewGuid().ToString()
                }
            };

            // Act
            var act = async () => await _service.AddOrder(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.AddEntity(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task AddOrder_WhenSkillGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new AddOrderRequest
            {
                Order = new OrderApi
                {
                    Name = "Order",
                    MasterGuid = Guid.NewGuid().ToString(),
                    SkillGuid = "invalid-guid"
                }
            };

            // Act
            var act = async () => await _service.AddOrder(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.AddEntity(It.IsAny<Order>()), Times.Never);
        }

        #endregion

        #region DelOrder Tests

        [Fact]
        public async Task DelOrder_WhenValid_DeletesEntityAndPublishesDelOrderEvent()
        {
            // Arrange
            var orderGuid = Guid.NewGuid();
            var offerGuid = Guid.NewGuid();

            var request = new DelOrderRequest
            {
                Order = new OrderApi
                {
                    Guid = orderGuid.ToString(),
                    OfferGuid = offerGuid.ToString(),
                    Name = "Cancelled Order"
                }
            };

            _mockRepo.Setup(r => r.DelEntity(orderGuid)).Returns(true);

            // Act
            var reply = await _service.DelOrder(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.DelEntity(orderGuid), Times.Once);

            _mockEventBus.Verify(b => b.Publish(It.Is<DelOrderEvent>(e =>
                e.Name == "Cancelled Order" &&
                e.Guid == orderGuid &&
                e.OfferGuid == offerGuid)), Times.Once);
        }

        [Fact]
        public async Task DelOrder_WhenRepositoryFails_DoesNotPublishEventAndReturnsFalse()
        {
            // Arrange
            var orderGuid = Guid.NewGuid();
            var request = new DelOrderRequest
            {
                Order = new OrderApi
                {
                    Guid = orderGuid.ToString(),
                    OfferGuid = Guid.NewGuid().ToString(),
                    Name = "Order"
                }
            };

            _mockRepo.Setup(r => r.DelEntity(orderGuid)).Returns(false);

            // Act
            var reply = await _service.DelOrder(request, _callContext);

            // Assert
            reply.Result.Should().BeFalse();
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelOrderEvent>()), Times.Never);
        }

        [Fact]
        public async Task DelOrder_WhenGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var request = new DelOrderRequest
            {
                Order = new OrderApi
                {
                    Guid = "invalid-guid",
                    OfferGuid = Guid.NewGuid().ToString(),
                    Name = "Order"
                }
            };

            // Act
            var act = async () => await _service.DelOrder(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region Unimplemented Base Methods Tests

        [Fact]
        public async Task GetOrder_CallsBaseMethod_ThrowsRpcException()
        {
            // Act
            var act = async () => await _service.GetOrder(new GetOrderRequest(), _callContext);

            // Assert
            var ex = await act.Should().ThrowAsync<RpcException>();
            ex.Which.StatusCode.Should().Be(StatusCode.Unimplemented);
        }

        [Fact]
        public async Task GetOrders_CallsBaseMethod_ThrowsRpcException()
        {
            // Act
            var act = async () => await _service.GetOrders(new GetOrdersRequest(), _callContext);

            // Assert
            var ex = await act.Should().ThrowAsync<RpcException>();
            ex.Which.StatusCode.Should().Be(StatusCode.Unimplemented);
        }

        #endregion
    }
}
