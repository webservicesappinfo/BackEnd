using FluentAssertions;
using MediatR.Users.Register;
using Auth.Models.DTO;
using Xunit;

namespace AuthService.Tests.MediatR.Users.Register
{
    public class RegisterUserCommandTests
    {
        [Fact]
        public void Constructor_ShouldSetRegisterRequest()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                Password = "Password123!",
                PasswordConfirmation = "Password123!"
            };

            // Act
            var command = new RegisterUserCommand(registerDto);

            // Assert
            command.registerRequest.Should().NotBeNull();
            command.registerRequest.Name.Should().Be("John");
            command.registerRequest.Surname.Should().Be("Doe");
            command.registerRequest.Email.Should().Be("john@example.com");
            command.registerRequest.Password.Should().Be("Password123!");
            command.registerRequest.PasswordConfirmation.Should().Be("Password123!");
        }
    }
}
