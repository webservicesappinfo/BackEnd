using FluentResults;
using MediatR;
using Auth.Models.DTO;

namespace Auth.MediatR.Users.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestDto RefreshTokenRequest)
        : IRequest<Result<AuthResponseDto>>;
}