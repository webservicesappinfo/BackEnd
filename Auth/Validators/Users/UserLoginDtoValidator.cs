using FluentValidation;
using Auth.Models.DTO;
using Auth.Resources;
using Auth.Validators;

namespace Auth.Validators.Users
{
    public class UserLoginDtoValidator : BaseUserValidator<UserLoginDto>
    {
        private const int MaxLoginLength = 20;
        private const int MaxPasswordLength = 20;
        private const int MinPasswordLength = 8;

        public UserLoginDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            ApplyLoginRules(x => x.Login, MaxLoginLength);

            ApplyPasswordRules(x => x.Password, MinPasswordLength, MaxPasswordLength,
                 ErrorMessages.PasswordIsRequired,
                 ErrorMessages.PasswordMustBeAtLeastCharacters,
                 ErrorMessages.PasswordMustNotExceedCharacters);
        }
    }
}