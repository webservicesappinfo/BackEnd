using AuthService.Protos;
using AuthService.Services;
using FluentAssertions;
using FluentResults;
using Globals.Tests;
using Google.Apis.Auth;
using MediatR;
using MediatR.Users.Register;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
using Moq;
using Auth.MediatR.Users.Login;
using Auth.MediatR.Users.LoginGoogle;
using Auth.MediatR.Users.Logout;
using Auth.Models.DTO;
using Auth.Models.DTO.Users;
using Auth.Models.MediatR.Users.ChangePassword;
using Auth.Services.Interfaces.Users;
using Xunit;

namespace AuthService.Tests.Services
{
    public class AuthServiceImpTests
    {
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<ILogger<AuthServiceImp>> _loggerMock = new();
        private readonly Mock<IGoogleAuthService> _googleAuthServiceMock = new();
        private readonly AuthServiceImp _service;
        private readonly TestServerCallContext _context;

        public AuthServiceImpTests()
        {
            _service = new AuthServiceImp(
                _mediatorMock.Object,
                _loggerMock.Object,
                _googleAuthServiceMock.Object);

            _context = TestServerCallContext.Create();
        }

        // ==========================================
        // 1. Register Tests
        // ==========================================

        [Fact]
        public async Task Register_WhenValid_ReturnsSuccessReply()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@test.com",
                Password = "Password123!",
                PasswordConfirmation = "Password123!"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "jwt-token-xyz",
                RefreshToken = "refresh-token-xyz",
                ExpireAt = DateTime.UtcNow.AddHours(1),
                User = new UserDto
                {
                    Id = 10,
                    Name = "John",
                    Surname = "Doe",
                    Email = "john@test.com",
                    Login = "johndoe",
                    Role = UserRole.User
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(authResponse));

            // Act
            var reply = await _service.Register(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeTrue();
            reply.Token.Should().Be("jwt-token-xyz");
            reply.RefreshToken.Should().Be("refresh-token-xyz");
            reply.User.Should().NotBeNull();
            reply.User.Id.Should().Be(10);
            reply.User.Email.Should().Be("john@test.com");
            reply.User.Role.Should().Be(UserRole.User.ToString());
        }

        [Fact]
        public async Task Register_WhenMediatorFails_ReturnsFailedReply()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "John",
                Surname = "Doe",
                Email = "existing@test.com",
                Password = "Password123!",
                PasswordConfirmation = "Password123!"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<AuthResponseDto>("User already exists"));

            // Act
            var reply = await _service.Register(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Contain("User already exists");
        }

        // ==========================================
        // 2. Login Tests
        // ==========================================

        [Fact]
        public async Task Login_WhenCredentialsValid_ReturnsSuccessReply()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "johndoe",
                Password = "Password123!"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "jwt-login-token",
                RefreshToken = "refresh-login-token",
                ExpireAt = DateTime.UtcNow.AddHours(2),
                User = new UserDto
                {
                    Id = 5,
                    Name = "John",
                    Surname = "Doe",
                    Email = "john@test.com",
                    Login = "johndoe",
                    Role = UserRole.Admin
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(authResponse));

            // Act
            var reply = await _service.Login(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeTrue();
            reply.Token.Should().Be("jwt-login-token");
            reply.User.Role.Should().Be(UserRole.Admin.ToString());
        }

        [Fact]
        public async Task Login_WhenInvalidCredentials_ReturnsFailedReply()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "johndoe",
                Password = "WrongPassword"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<AuthResponseDto>("Invalid login or password."));

            // Act
            var reply = await _service.Login(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Contain("Invalid login or password.");
        }

        // ==========================================
        // 3. GoogleLogin Tests
        // ==========================================

        [Fact]
        public async Task GoogleLogin_WhenTokenInvalid_ReturnsFailedReply()
        {
            // Arrange
            var request = new MobileApiGetway.GoogleLoginRequest { IdToken = "invalid-token" };

            _googleAuthServiceMock
                .Setup(g => g.ValidateTokenAsync("invalid-token"))
                .ReturnsAsync((GoogleJsonWebSignature.Payload?)null);

            // Act
            var reply = await _service.GoogleLogin(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Be("Invalid Google Token");
        }

        [Fact]
        public async Task GoogleLogin_WhenValid_ReturnsSuccessReply()
        {
            // Arrange
            var request = new MobileApiGetway.GoogleLoginRequest { IdToken = "valid-google-token" };

            var payload = new GoogleJsonWebSignature.Payload
            {
                Email = "google@test.com",
                GivenName = "Google",
                FamilyName = "User"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "google-jwt",
                RefreshToken = "google-refresh",
                ExpireAt = DateTime.UtcNow.AddHours(1),
                User = new UserDto
                {
                    Id = 20,
                    Email = "google@test.com",
                    Name = "Google",
                    Surname = "User",
                    Role = UserRole.MainAdministrator
                }
            };

            _googleAuthServiceMock
                .Setup(g => g.ValidateTokenAsync("valid-google-token"))
                .ReturnsAsync(payload);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GoogleLoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(authResponse));

            // Act
            var reply = await _service.GoogleLogin(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeTrue();
            reply.Token.Should().Be("google-jwt");
            reply.User.Email.Should().Be("google@test.com");
        }

        // ==========================================
        // 4. ChangePassword Tests
        // ==========================================

        [Fact]
        public async Task ChangePassword_WhenUserIdInvalid_ReturnsFailedReply()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                UserId = 0,
                CurrentPassword = "old",
                NewPassword = "new",
                ConfirmNewPassword = "new"
            };

            // Act
            var reply = await _service.ChangePassword(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Be("Invalid user ID.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ChangePassword_WhenValid_ReturnsSuccessReply()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                UserId = 1,
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(Unit.Value));

            // Act
            var reply = await _service.ChangePassword(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ChangePassword_WhenMediatorFails_ReturnsFailedReply()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                UserId = 1,
                CurrentPassword = "WrongPassword!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<Unit>("Password mismatch"));

            // Act
            var reply = await _service.ChangePassword(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Contain("Password mismatch");
        }

        // ==========================================
        // 5. Logout Tests
        // ==========================================

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Logout_WhenRefreshTokenEmpty_ReturnsFailedReply(string token)
        {
            // Arrange
            var request = new LogoutRequest { RefreshToken = token };

            // Act
            var reply = await _service.Logout(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Be("Refresh token is required.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<LogoutUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Logout_WhenValid_ReturnsSuccessReply()
        {
            // Arrange
            var request = new LogoutRequest { RefreshToken = "valid-refresh-token" };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LogoutUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(Unit.Value));

            // Act
            var reply = await _service.Logout(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Logout_WhenMediatorFails_ReturnsFailedReply()
        {
            // Arrange
            var request = new LogoutRequest { RefreshToken = "revoked-token" };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LogoutUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<Unit>("Failed to revoke session."));

            // Act
            var reply = await _service.Logout(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.IsSuccess.Should().BeFalse();
            reply.ErrorMessage.Should().Contain("Failed to revoke session.");
        }
    }
}
