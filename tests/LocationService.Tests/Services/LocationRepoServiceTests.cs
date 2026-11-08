using EventBus.Abstractions;
using FluentAssertions;
using LocationService.Abstractions;
using LocationService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LocationService.Tests.Services
{
    public class LocationRepoServiceTests
    {
        [Fact]
        public void LocationRepoService_CanBeConstructed_AndImplementsILocationRepoService()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<LocationRepoService>>();
            var mockEventBus = new Mock<IEventBus>();

            // Act
            var service = new LocationRepoService(mockLogger.Object, mockEventBus.Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<ILocationRepoService>();
        }
    }
}
