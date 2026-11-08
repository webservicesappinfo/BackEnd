using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using FluentAssertions;
using Globals.Tests;
using LocationService.Abstractions;
using LocationService.Protos;
using LocationService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LocationService.Tests.Services
{
    public class LocationRepoImpTests
    {
        private readonly Mock<ILogger<LocationRepoImp>> _mockLogger;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<ILocationRepoService> _mockRepo;
        private readonly TestServerCallContext _callContext;
        private readonly LocationRepoImp _service;

        public LocationRepoImpTests()
        {
            _mockLogger = new Mock<ILogger<LocationRepoImp>>();
            _mockEventBus = new Mock<IEventBus>();
            _mockRepo = new Mock<ILocationRepoService>();
            _callContext = new TestServerCallContext();

            _service = new LocationRepoImp(
                _mockLogger.Object,
                _mockEventBus.Object,
                _mockRepo.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenValidArguments_InitializesCorrectly()
        {
            // Act
            var service = new LocationRepoImp(
                _mockLogger.Object,
                _mockEventBus.Object,
                _mockRepo.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region GetUserLocation Tests

        [Fact]
        public async Task GetUserLocation_WhenUserLocationFound_ReturnsReplyWithCoordinates()
        {
            // Arrange
            var userGuid = Guid.NewGuid().ToString();
            var expectedLat = "50.4501";
            var expectedLng = "30.5234";

            var request = new GetUserLocationRequest
            {
                Guid = userGuid
            };

            var outLat = expectedLat;
            var outLng = expectedLng;
            _mockRepo.Setup(r => r.GetUserLocation(userGuid, out outLat, out outLng));

            // Act
            var reply = await _service.GetUserLocation(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.ForGuid.Should().Be(userGuid);
            reply.Lat.Should().Be(expectedLat);
            reply.Lng.Should().Be(expectedLng);

            _mockRepo.Verify(r => r.GetUserLocation(userGuid, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny), Times.Once);
        }

        [Fact]
        public async Task GetUserLocation_WhenUserLocationEmpty_ReturnsReplyWithEmptyCoordinates()
        {
            // Arrange
            var userGuid = Guid.NewGuid().ToString();
            var request = new GetUserLocationRequest
            {
                Guid = userGuid
            };

            var emptyLat = string.Empty;
            var emptyLng = string.Empty;
            _mockRepo.Setup(r => r.GetUserLocation(userGuid, out emptyLat, out emptyLng));

            // Act
            var reply = await _service.GetUserLocation(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.ForGuid.Should().Be(userGuid);
            reply.Lat.Should().BeEmpty();
            reply.Lng.Should().BeEmpty();

            _mockRepo.Verify(r => r.GetUserLocation(userGuid, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny), Times.Once);
        }

        [Fact]
        public async Task GetUserLocation_WhenGuidEmpty_CallsRepoAndReturnsReply()
        {
            // Arrange
            var request = new GetUserLocationRequest
            {
                Guid = string.Empty
            };

            var emptyLat = string.Empty;
            var emptyLng = string.Empty;
            _mockRepo.Setup(r => r.GetUserLocation(string.Empty, out emptyLat, out emptyLng));

            // Act
            var reply = await _service.GetUserLocation(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.ForGuid.Should().BeEmpty();
            reply.Lat.Should().BeEmpty();
            reply.Lng.Should().BeEmpty();

            _mockRepo.Verify(r => r.GetUserLocation(string.Empty, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny), Times.Once);
        }

        [Fact]
        public async Task GetUserLocation_WhenRequestIsNull_ThrowsNullReferenceException()
        {
            // Act
            var act = async () => await _service.GetUserLocation(null!, _callContext);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
            _mockRepo.Verify(r => r.GetUserLocation(It.IsAny<string>(), out It.Ref<string>.IsAny, out It.Ref<string>.IsAny), Times.Never);
        }

        #endregion

        #region SetUserLocation Tests

        [Fact]
        public async Task SetUserLocation_WhenValidCoordinates_ReturnsSuccess()
        {
            // Arrange
            var userGuid = Guid.NewGuid().ToString();
            var lat = "40.7128";
            var lng = "-74.0060";

            var request = new SetUserLocationRequest
            {
                ForGuid = userGuid,
                Lat = lat,
                Lng = lng
            };

            _mockRepo.Setup(r => r.SetUserLocation(userGuid, lat, lng))
                .Returns(true);

            // Act
            var reply = await _service.SetUserLocation(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSet.Should().BeTrue();

            _mockRepo.Verify(r => r.SetUserLocation(userGuid, lat, lng), Times.Once);
        }

        [Fact]
        public async Task SetUserLocation_WhenRepositoryFails_ReturnsFalse()
        {
            // Arrange
            var userGuid = Guid.NewGuid().ToString();
            var lat = "invalid_lat";
            var lng = "invalid_lng";

            var request = new SetUserLocationRequest
            {
                ForGuid = userGuid,
                Lat = lat,
                Lng = lng
            };

            _mockRepo.Setup(r => r.SetUserLocation(userGuid, lat, lng))
                .Returns(false);

            // Act
            var reply = await _service.SetUserLocation(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSet.Should().BeFalse();

            _mockRepo.Verify(r => r.SetUserLocation(userGuid, lat, lng), Times.Once);
        }

        [Fact]
        public async Task SetUserLocation_WhenCoordinatesAreEmpty_CallsRepoAndReturnsResult()
        {
            // Arrange
            var userGuid = Guid.NewGuid().ToString();
            var request = new SetUserLocationRequest
            {
                ForGuid = userGuid,
                Lat = string.Empty,
                Lng = string.Empty
            };

            _mockRepo.Setup(r => r.SetUserLocation(userGuid, string.Empty, string.Empty))
                .Returns(false);

            // Act
            var reply = await _service.SetUserLocation(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSet.Should().BeFalse();

            _mockRepo.Verify(r => r.SetUserLocation(userGuid, string.Empty, string.Empty), Times.Once);
        }

        [Fact]
        public async Task SetUserLocation_WhenRequestIsNull_ThrowsNullReferenceException()
        {
            // Act
            var act = async () => await _service.SetUserLocation(null!, _callContext);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
            _mockRepo.Verify(r => r.SetUserLocation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
