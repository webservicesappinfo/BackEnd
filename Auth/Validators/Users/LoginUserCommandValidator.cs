using FluentValidation;
using Auth.MediatR.Users.Login;

namespace Auth.Validators.Users;

public class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.loginRequest)
            .SetValidator(new UserLoginDtoValidator());
    }
}