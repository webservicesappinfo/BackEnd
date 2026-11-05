using FluentAssertions;
using Auth.Models.Entities;
using Auth.Extensions;
using Xunit;

namespace AuthService.Tests.Extensions
{
    public class UserExtensionsTests
    {
        private static User CreateTestUser(string? stamp = null)
        {
            return new User
            {
                Name = "John",
                Surname = "Doe",
                SecurityStamp = stamp
            };
        }

        [Fact]
        public void EnsureSecurityStamp_ShouldSetNewStamp_WhenStampIsNullOrEmpty()
        {
            // Arrange
            var user = CreateTestUser(stamp: null);

            // Act
            user.EnsureSecurityStamp();

            // Assert
            user.SecurityStamp.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void EnsureSecurityStamp_ShouldNotChangeStamp_WhenStampAlreadyExists()
        {
            // Arrange
            var existingStamp = "some-fixed-stamp";
            var user = CreateTestUser(stamp: existingStamp);

            // Act
            user.EnsureSecurityStamp();

            // Assert
            user.SecurityStamp.Should().Be(existingStamp);
        }
    }
}
