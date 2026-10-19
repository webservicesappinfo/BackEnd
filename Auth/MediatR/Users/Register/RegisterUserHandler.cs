using Auth.Data;
using Auth.Models.DTO;
using FluentResults;
using MediatR;
using MediatR.Users.Register;

namespace Auth.MediatR.Users.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly AuthDbContext _context;

        public RegisterUserHandler(AuthDbContext context)
        {
            _context = context;
        }

        public Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
