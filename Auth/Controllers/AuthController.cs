using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok("Auth");
        }
    }
}
