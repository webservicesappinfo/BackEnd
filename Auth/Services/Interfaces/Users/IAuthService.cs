using Auth.Models.DTO;
using Auth.Models.Entities;

namespace Auth.Services.Interfaces.Users
{
    public interface IAuthService
    {
        Task<AuthResponseDto> CreateLoginResultAsync(User user);
    }
}