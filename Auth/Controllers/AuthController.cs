using Auth.Extensions;
using Auth.MediatR.Users.Login;
using Auth.MediatR.Users.LoginGoogle;
using Auth.MediatR.Users.Logout;
using Auth.MediatR.Users.RefreshToken;
using Auth.Models.DTO;
using Auth.Models.DTO.Users;
using Auth.Models.MediatR.Users.ChangePassword;
using Auth.Services.Interfaces.Users;
using MediatR;
using MediatR.Users.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthController(IMediator mediator, IGoogleAuthService googleAuthService)
        {
            _mediator = mediator;
            _googleAuthService = googleAuthService;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var result = await _mediator.Send(new RegisterUserCommand(request));
            return this.ToActionResult(result);
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var payload = await _googleAuthService.ValidateTokenAsync(request.IdToken);

                if (payload == null)
                    return Unauthorized("Invalid Google Token");

                var loginDto = new GoogleLoginRequestDto
                {
                    Email = payload.Email,
                    Name = payload.GivenName,
                    Surname = payload.FamilyName
                };

                var result = await _mediator.Send(new GoogleLoginCommand(loginDto));
                return this.ToActionResult(result);
            }
            catch (Exception)
            {
                return Unauthorized("Invalid Google Token");
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var result = await _mediator.Send(new LoginUserCommand(request));
            return this.ToActionResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "X-Refresh-Token")] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Refresh token missing in X-Refresh-Token header.");

            var result = await _mediator.Send(new LogoutUserCommand(refreshToken));
            return this.ToActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(request));
            return this.ToActionResult(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request, CancellationToken cancellationToken = default)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new ChangePasswordCommand(userId, request), cancellationToken);
            return this.ToActionResult(result);
        }

        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok("Auth");
        }
    }
}
