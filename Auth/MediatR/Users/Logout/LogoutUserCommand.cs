using FluentResults;
using MediatR;

namespace Auth.MediatR.Users.Logout
{
    public record LogoutUserCommand(string RefreshToken) : IRequest<Result<Unit>>;
}
