using Auth.Models.DTO;
using Auth.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Services.Interfaces.Users
{
    public interface IJwtTokenService
    {
        JwtSecurityToken GenerateToken(User user);
    }
}
