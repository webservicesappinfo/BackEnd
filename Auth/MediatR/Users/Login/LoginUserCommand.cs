using FluentResults;
using MediatR;
using Auth.Models.DTO;


namespace Auth.MediatR.Users.Login
{
    public record LoginUserCommand(UserLoginDto loginRequest)
        : IRequest<Result<AuthResponseDto>>;
}
