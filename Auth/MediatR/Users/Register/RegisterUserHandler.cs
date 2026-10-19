using Auth.Data;
using Auth.Extensions;
using Auth.Models.DTO;
using Auth.Models.Entities;
using Auth.Services.Interfaces.Users;
using AutoMapper;
using FluentResults;
using MediatR;
using MediatR.Users.Register;
using Microsoft.AspNetCore.Identity;

namespace Auth.MediatR.Users.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public RegisterUserHandler(AuthDbContext context, UserManager<User> userManager, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.registerRequest.Email);
            if (existingUser != null)
            {
                return Result.Fail<AuthResponseDto>("User already exists");
            }

            var user = _mapper.Map<User>(request.registerRequest);
            user.UserName = request.registerRequest.Email;
            user.EnsureSecurityStamp();

            var result = await _userManager.CreateAsync(user, request.registerRequest.Password);
            if (!result.Succeeded)
            {
                return Result.Fail<AuthResponseDto>(result.Errors.Select(e => e.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, UserRole.Moderator.ToString());
            if (!roleResult.Succeeded)
            {
                return Result.Fail<AuthResponseDto>(roleResult.Errors.Select(e => e.Description));
            }

            await _context.SaveChangesAsync(cancellationToken);

            var registrResult = await _authService.CreateLoginResultAsync(user);

            return Result.Ok(registrResult);
        }
    }
}
