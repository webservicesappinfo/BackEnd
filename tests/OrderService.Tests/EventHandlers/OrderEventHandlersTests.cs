using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus.Events.ServicesEvents.OfferEvents;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Abstractions;
using OrderService.EventHandlers;
using OrderService.Models;
using Xunit;
using Order = OrderService.Models.Order;

namespace OrderService.Tests.EventHandlers
{
    public class OrderEventHandlersTests
    {
        private readonly Mock<IOrderRepoService> _mockRepo;

        public OrderEventHandlersTests()
        {
            _mockRepo = new Mock<IOrderRepoService>();
        }

        #region DelOfferEH Tests

        [Fact]
        public async Task DelOfferEH_Handle_CallsOnDelOfferEHonRepositoryAndReturnsResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelOfferEvent>>();
            var handler = new DelOfferEH(mockLogger.Object, _mockRepo.Object);

            var offerGuid = Guid.NewGuid();
            var masterGuid = Guid.NewGuid();
            var @event = new DelOfferEvent("Haircut Service", offerGuid, masterGuid);

            _mockRepo.Setup(r => r.OnDelOfferEH(offerGuid, masterGuid)).Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.OnDelOfferEH(offerGuid, masterGuid), Times.Once);
        }

        #endregion

        #region DelUserEH Tests

        [Fact]
        public async Task DelUserEH_Handle_WhenUserIsMaster_DeletesMatchingOrders()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            var otherGuid = Guid.NewGuid();

            var order1 = new Order { Guid = Guid.NewGuid(), MasterGuid = userGuid, UserGuid = otherGuid };
            var order2 = new Order { Guid = Guid.NewGuid(), MasterGuid = otherGuid, UserGuid = otherGuid };

            _mockRepo.Setup(r => r.GetEntities()).Returns(new List<Order> { order1, order2 });
            _mockRepo.Setup(r => r.DelEntity(It.IsAny<Guid>())).Returns(true);

            var @event = new DelUserEvent("MasterUser", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(order1.Guid), Times.Once);
            _mockRepo.Verify(r => r.DelEntity(order2.Guid), Times.Never);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenUserIsClient_DeletesMatchingOrders()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            var otherGuid = Guid.NewGuid();

            var order1 = new Order { Guid = Guid.NewGuid(), MasterGuid = otherGuid, UserGuid = userGuid };
            var order2 = new Order { Guid = Guid.NewGuid(), MasterGuid = otherGuid, UserGuid = otherGuid };

            _mockRepo.Setup(r => r.GetEntities()).Returns(new List<Order> { order1, order2 });
            _mockRepo.Setup(r => r.DelEntity(It.IsAny<Guid>())).Returns(true);

            var @event = new DelUserEvent("ClientUser", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(order1.Guid), Times.Once);
            _mockRepo.Verify(r => r.DelEntity(order2.Guid), Times.Never);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenUserHasNoMatchingOrders_DoesNotCallDelete()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            var otherGuid = Guid.NewGuid();

            var order1 = new Order { Guid = Guid.NewGuid(), MasterGuid = otherGuid, UserGuid = otherGuid };

            _mockRepo.Setup(r => r.GetEntities()).Returns(new List<Order> { order1 });

            var @event = new DelUserEvent("UnrelatedUser", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenGetEntitiesReturnsNull_HandlesSafelyWithoutThrowing()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetEntities()).Returns((List<Order>)null!);

            var @event = new DelUserEvent("User", userGuid.ToString(), "token123");

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DelUserEH_Handle_WhenGuidInvalid_ThrowsFormatException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DelUserEvent>>();
            var handler = new DelUserEH(mockLogger.Object, _mockRepo.Object);

            var @event = new DelUserEvent("User", "invalid-guid", "token123");

            // Act
            var act = async () => await handler.Handle(@event);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
        }

        #endregion
    }
}
