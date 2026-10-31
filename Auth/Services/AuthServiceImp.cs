using Auth.Models.DTO;
using AuthService.Protos;
using Grpc.Core;
using MediatR;
using Auth.MediatR.Users.Login;
using Auth.MediatR.Users.LoginGoogle;
using Auth.MediatR.Users.Logout;
using MediatR.Users.Register;
using MobileApiGetway;
using Auth.Models.DTO.Users;
using Auth.Models.MediatR.Users.ChangePassword;
using Auth.Services.Interfaces.Users;

namespace AuthService.Services
{
    public class AuthServiceImp : Protos.Auth.AuthBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthServiceImp> _logger;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthServiceImp(IMediator mediator, ILogger<AuthServiceImp> logger, IGoogleAuthService googleAuthService)
        {
            _mediator = mediator;
            _logger = logger;
            _googleAuthService = googleAuthService;
        }

        public override async Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC Register request received for email: {Email}", request.Email);

            var registerDto = new UserRegisterDto
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Password = request.Password,
                PasswordConfirmation = request.PasswordConfirmation
            };

            var result = await _mediator.Send(new RegisterUserCommand(registerDto));

            var reply = new RegisterReply();

            if (result.IsSuccess)
            {
                reply.IsSuccess = true;
                reply.Token = result.Value.Token ?? string.Empty;
                reply.RefreshToken = result.Value.RefreshToken ?? string.Empty;
                reply.ExpireAt = result.Value.ExpireAt.ToString("o");

                if (result.Value.User != null)
                {
                    reply.User = new AuthUser
                    {
                        Id = result.Value.User.Id,
                        Name = result.Value.User.Name ?? string.Empty,
                        Surname = result.Value.User.Surname ?? string.Empty,
                        Email = result.Value.User.Email ?? string.Empty,
                        Login = result.Value.User.Login ?? string.Empty,
                        Role = result.Value.User.Role.ToString()
                    };
                }
            }
            else
            {
                reply.IsSuccess = false;
                reply.ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Message));
            }

            return reply;
        }

        public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC Login request received for login: {Login}", request.Login);

            var loginDto = new UserLoginDto
            {
                Login = request.Login,
                Password = request.Password
            };

            var result = await _mediator.Send(new LoginUserCommand(loginDto));

            var reply = new LoginReply();

            if (result.IsSuccess)
            {
                reply.IsSuccess = true;
                reply.Token = result.Value.Token ?? string.Empty;
                reply.RefreshToken = result.Value.RefreshToken ?? string.Empty;
                reply.ExpireAt = result.Value.ExpireAt.ToString("o");

                if (result.Value.User != null)
                {
                    reply.User = new AuthUser
                    {
                        Id = result.Value.User.Id,
                        Name = result.Value.User.Name ?? string.Empty,
                        Surname = result.Value.User.Surname ?? string.Empty,
                        Email = result.Value.User.Email ?? string.Empty,
                        Login = result.Value.User.Login ?? string.Empty,
                        Role = result.Value.User.Role.ToString()
                    };
                }
            }
            else
            {
                reply.IsSuccess = false;
                reply.ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Message));
            }

            return reply;
        }

        public override async Task<GoogleLoginReply> GoogleLogin(MobileApiGetway.GoogleLoginRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC GoogleLogin request received");

            try
            {
                var payload = await _googleAuthService.ValidateTokenAsync(request.IdToken);

                if (payload == null)
                {
                    return new GoogleLoginReply
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid Google Token"
                    };
                }

                var loginDto = new GoogleLoginRequestDto
                {
                    Email = payload.Email,
                    Name = payload.GivenName,
                    Surname = payload.FamilyName
                };

                var result = await _mediator.Send(new GoogleLoginCommand(loginDto), context.CancellationToken);

                var reply = new GoogleLoginReply();

                if (result.IsSuccess)
                {
                    reply.IsSuccess = true;
                    reply.Token = result.Value.Token ?? string.Empty;
                    reply.RefreshToken = result.Value.RefreshToken ?? string.Empty;
                    reply.ExpireAt = result.Value.ExpireAt.ToString("o");

                    if (result.Value.User != null)
                    {
                        reply.User = new AuthUser
                        {
                            Id = result.Value.User.Id,
                            Name = result.Value.User.Name ?? string.Empty,
                            Surname = result.Value.User.Surname ?? string.Empty,
                            Email = result.Value.User.Email ?? string.Empty,
                            Login = result.Value.User.Login ?? string.Empty,
                            Role = result.Value.User.Role.ToString()
                        };
                    }
                }
                else
                {
                    reply.IsSuccess = false;
                    reply.ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Message));
                }

                return reply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC Google Login");
                return new GoogleLoginReply
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid Google Token"
                };
            }
        }

        public override async Task<ChangePasswordReply> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC ChangePassword request received for userId: {UserId}", request.UserId);

            if (request.UserId <= 0)
            {
                return new ChangePasswordReply
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid user ID."
                };
            }

            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                ConfirmNewPassword = request.ConfirmNewPassword
            };

            var result = await _mediator.Send(new ChangePasswordCommand(request.UserId, changePasswordDto), context.CancellationToken);

            var reply = new ChangePasswordReply();

            if (result.IsSuccess)
            {
                reply.IsSuccess = true;
            }
            else
            {
                reply.IsSuccess = false;
                reply.ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Message));
            }

            return reply;
        }

        public override async Task<LogoutReply> Logout(LogoutRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC Logout request received");

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return new LogoutReply
                {
                    IsSuccess = false,
                    ErrorMessage = "Refresh token is required."
                };
            }

            var result = await _mediator.Send(new LogoutUserCommand(request.RefreshToken));

            var reply = new LogoutReply();

            if (result.IsSuccess)
            {
                reply.IsSuccess = true;
            }
            else
            {
                reply.IsSuccess = false;
                reply.ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Message));
            }

            return reply;
        }
    }
}
