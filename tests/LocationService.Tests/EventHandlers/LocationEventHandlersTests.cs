using System;
using System.Threading.Tasks;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using EventBus.ServicesEvents.MobileClientEvents;
using FluentAssertions;
using LocationService.Abstractions;
using LocationService.EventHandlers;
using LocationService.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LocationService.Tests.EventHandlers
{
    public class LocationEventHandlersTests
    {
        private readonly Mock<ILocationRepoService> _mockRepo;

        public LocationEventHandlersTests()
        {
            _mockRepo = new Mock<ILocationRepoService>();
        }

        #region AddUserEH Tests

        [Fact]
        public async Task AddUserEH_Handle_CallsAddUserOnRepositoryWithMatchingUser()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddUserEH>>();
            var handler = new AddUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid().ToString();
            var @event = new AddUserEvent("Alice Johnson", userGuid, "token-xyz");

            _mockRepo.Setup(r => r.AddUser(It.IsAny<LocationUser>()))
                .Returns(true);

            // Act
            await handler.Handle(@event);

            // Assert
            _mockRepo.Verify(r => r.AddUser(It.Is<LocationUser>(u =>
                u.Name == "Alice Johnson" &&
                u.Guid == userGuid)), Times.Once);
        }

        [Fact]
        public async Task AddUserEH_Handle_WhenRepositoryReturnsFalse_CompletesSuccessfully()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddUserEH>>();
            var handler = new AddUserEH(mockLogger.Object, _mockRepo.Object);

            var userGuid = Guid.NewGuid().ToString();
            var @event = new AddUserEvent("Bob Smith", userGuid, "token-abc");

            _mockRepo.Setup(r => r.AddUser(It.IsAny<LocationUser>()))
                .Returns(false);

            // Act
            var task = handler.Handle(@event);
            await task;

            // Assert
            task.IsCompletedSuccessfully.Should().BeTrue();
            _mockRepo.Verify(r => r.AddUser(It.Is<LocationUser>(u =>
                u.Name == "Bob Smith" &&
                u.Guid == userGuid)), Times.Once);
        }

        [Fact]
        public async Task AddUserEH_Handle_WhenEventIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<AddUserEH>>();
            var handler = new AddUserEH(mockLogger.Object, _mockRepo.Object);

            // Act
            var act = async () => await handler.Handle(null!);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
            _mockRepo.Verify(r => r.AddUser(It.IsAny<LocationUser>()), Times.Never);
        }

        #endregion

        #region TestBusEventHandler Tests

        [Fact]
        public async Task TestBusEventHandler_Handle_CompletesSuccessfully()
        {
            // Arrange
            var handler = new TestBusEventHandler();
            var @event = new TestBusEvent { Msg = "Ping from integration test" };

            // Act
            var task = handler.Handle(@event);
            await task;

            // Assert
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task TestBusEventHandler_Handle_WhenMsgIsNull_CompletesSuccessfully()
        {
            // Arrange
            var handler = new TestBusEventHandler();
            var @event = new TestBusEvent { Msg = null! };

            // Act
            var task = handler.Handle(@event);
            await task;

            // Assert
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task TestBusEventHandler_Handle_WhenEventIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var handler = new TestBusEventHandler();

            // Act
            var act = async () => await handler.Handle(null!);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        #endregion
    }
}
