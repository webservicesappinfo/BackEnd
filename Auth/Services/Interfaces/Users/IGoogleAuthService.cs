using Google.Apis.Auth;

namespace Auth.Services.Interfaces.Users
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload?> ValidateTokenAsync(string idToken);
    }
}
