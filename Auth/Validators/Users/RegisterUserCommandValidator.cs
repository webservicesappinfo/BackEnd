using FluentValidation;
using MediatR.Users.Register;

namespace Auth.Validators.Users;

public class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.registerRequest)
            .SetValidator(new UserRegisterDtoValidator());
    }
}