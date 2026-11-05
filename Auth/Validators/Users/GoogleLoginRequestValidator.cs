using FluentValidation;
using Auth.Models.DTO;
using Auth.Resources;

namespace Auth.Validators.Users
{
    public class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage(ErrorMessages.GoogleIDTokenIsRequired)
                .MinimumLength(100).WithMessage(ErrorMessages.InvalidTokenFormat);
        }
    }
}
