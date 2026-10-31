using FluentResults;
using MediatR;
using Auth.Models.DTO;

namespace Auth.MediatR.Users.LoginGoogle
{
    public record GoogleLoginCommand(GoogleLoginRequestDto googleLoginRequest)
      : IRequest<Result<AuthResponseDto>>;
}
