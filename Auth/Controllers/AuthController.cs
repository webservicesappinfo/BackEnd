using Auth.Extensions;
using Auth.MediatR.Users.Login;
using Auth.MediatR.Users.Logout;
using Auth.Models.DTO;
using MediatR;
using MediatR.Users.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var result = await _mediator.Send(new RegisterUserCommand(request));
            return this.ToActionResult(result);
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

        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok("Auth");
        }
    }
}
