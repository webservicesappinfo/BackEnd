using Auth.Models.DTO;
using AuthService.Protos;
using Grpc.Core;
using MediatR;
using Auth.MediatR.Users.Login;
using Auth.MediatR.Users.Logout;
using MediatR.Users.Register;
using MobileApiGetway;

namespace AuthService.Services
{
    public class AuthServiceImp : Protos.Auth.AuthBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthServiceImp> _logger;

        public AuthServiceImp(IMediator mediator, ILogger<AuthServiceImp> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
