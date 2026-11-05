using Auth.Models.DTO;
using Xunit;
using FluentAssertions;
using Auth.Validators.Users;

namespace AuthService.Tests.Models.DTO
{
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_Validation_ShouldFail_WhenNameIsTooLong()
        {
            var validator = new UserDtoValidator();
            var dto = new UserDto
            {
                Name = new string('A', 51),
                Surname = "Test",
                Email = "test@test.com",
                Login = "login"
            };

            var result = validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }
    }
}
