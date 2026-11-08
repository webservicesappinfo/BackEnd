using FluentAssertions;
using LocationService.Models;
using Xunit;

namespace LocationService.Tests.Models
{
    public class LocationUserTests
    {
        [Fact]
        public void LocationUser_CanSetAndGetProperties()
        {
            // Arrange
            var user = new LocationUser
            {
                Id = 42,
                Name = "John Doe",
                Guid = "abc-123-guid",
                Lat = 50.4501,
                Lng = 30.5234
            };

            // Assert
            user.Id.Should().Be(42);
            user.Name.Should().Be("John Doe");
            user.Guid.Should().Be("abc-123-guid");
            user.Lat.Should().Be(50.4501);
            user.Lng.Should().Be(30.5234);
        }

        [Fact]
        public void LocationUser_DefaultValues_AreExpected()
        {
            // Act
            var user = new LocationUser();

            // Assert
            user.Id.Should().Be(0);
            user.Name.Should().BeNull();
            user.Guid.Should().BeNull();
            user.Lat.Should().BeNull();
            user.Lng.Should().BeNull();
        }
    }
}
