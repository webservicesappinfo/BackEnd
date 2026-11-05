using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Auth.Data;
using Auth.MediatR.Users.Register;
using Auth.Models.DTO;
using Auth.Models.Entities;
using Auth.Services.Interfaces.Users;
using MediatR.Users.Register;
using Xunit;

namespace AuthService.Tests.MediatR.Users.Register;

public class RegisterUserHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _authServiceMock = new Mock<IAuthService>();

        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AuthDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserRegisterDto, User>();
            cfg.CreateMap<User, UserDto>();
        });
        _mapper = mapperConfig.CreateMapper();

        _handler = new RegisterUserHandler(
            _context,
            _userManagerMock.Object,
            _mapper,
            _authServiceMock.Object);
    }

    private static UserRegisterDto CreateValidDto() => new()
    {
        Name = "John",
        Surname = "Doe",
        Email = "john@test.com",
        Password = "Password123!",
        PasswordConfirmation = "Password123!"
    };

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        var dto = CreateValidDto();

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), UserRole.Moderator.ToString()))
            .ReturnsAsync(IdentityResult.Success);

        _authServiceMock.Setup(x => x.CreateLoginResultAsync(It.IsAny<User>()))
            .ReturnsAsync(new AuthResponseDto
            {
                Token = "jwt-token",
                RefreshToken = "refresh-token",
                User = new UserDto
                {
                    Email = dto.Email,
                    Name = dto.Name,
                    Surname = dto.Surname
                }
            });

        // Act
        var result = await _handler.Handle(new RegisterUserCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("jwt-token");
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), dto.Password), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), UserRole.Moderator.ToString()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserAlreadyExists()
    {
        // Arrange
        var dto = CreateValidDto();
        var existingUser = new User { Id = 1, Name = "Existing", Surname = "User" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(new RegisterUserCommand(dto), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User already exists");
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCreateUserFails()
    {
        // Arrange
        var dto = CreateValidDto();

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        var identityError = new IdentityError { Description = "Password too simple" };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _handler.Handle(new RegisterUserCommand(dto), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Password too simple");
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenAddToRoleFails()
    {
        // Arrange
        var dto = CreateValidDto();

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        var identityError = new IdentityError { Description = "Role does not exist" };
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), UserRole.Moderator.ToString()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _handler.Handle(new RegisterUserCommand(dto), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Role does not exist");
    }
}
