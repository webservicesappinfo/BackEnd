using Auth.Models.DTO;
using MediatR;
using MediatR.Users.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth.Extensions;

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

        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok("Auth");
        }
    }
}
