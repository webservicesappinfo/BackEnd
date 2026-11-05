using FluentAssertions;
using Auth.MediatR.Users.RefreshToken;
using Auth.Models.DTO;
using Xunit;

namespace AuthService.Tests.MediatR.Users.RefreshToken;

public class RefreshTokenCommandTests
{
    [Fact]
    public void RefreshTokenCommand_ShouldHaveCorrectProperties()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = "refresh-token"
        };

        // Act
        var command = new RefreshTokenCommand(request);

        // Assert
        command.RefreshTokenRequest.RefreshToken.Should().Be("refresh-token");
    }
}
