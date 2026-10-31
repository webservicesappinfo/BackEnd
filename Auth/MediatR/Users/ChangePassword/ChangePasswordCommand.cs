using FluentResults;
using MediatR;
using Auth.Models.DTO.Users;

namespace Auth.Models.MediatR.Users.ChangePassword
{
    public record ChangePasswordCommand(int UserId, ChangePasswordDto ChangePasswordRequest)
     : IRequest<Result<Unit>>;
}
