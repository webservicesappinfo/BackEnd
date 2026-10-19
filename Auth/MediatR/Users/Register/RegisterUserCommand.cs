using Auth.Models.DTO;
using FluentResults;

namespace MediatR.Users.Register
{
    public record RegisterUserCommand(UserRegisterDto registerRequest)
    : IRequest<Result<AuthResponseDto>>;
}
