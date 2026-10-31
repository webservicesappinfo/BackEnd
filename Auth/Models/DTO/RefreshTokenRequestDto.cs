using System.Diagnostics.CodeAnalysis;

namespace Auth.Models.DTO
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokenRequestDto
    {
        required public string RefreshToken { get; set; }
    }
}