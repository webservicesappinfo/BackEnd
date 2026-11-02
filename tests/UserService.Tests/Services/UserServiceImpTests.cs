using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using FluentAssertions;
using Globals.Tests;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Abstractions;
using UserService.Models;
using UserService.Services;
using Xunit;

namespace UserService.Tests.Services
{
    public class UserServiceImpTests
    {
        private readonly Mock<ILogger<UserServiceImp>> _mockLogger = new();
        private readonly Mock<IEventBus> _mockEventBus = new();
        private readonly Mock<IUserRepoService> _mockUserRepo = new();

        private readonly UserServiceImp _service;
        private readonly TestServerCallContext _context;

        public UserServiceImpTests()
        {
            _service = new UserServiceImp(_mockLogger.Object, _mockEventBus.Object, _mockUserRepo.Object);
            _context = TestServerCallContext.Create();
        }

        // ==========================================
        // 1. AddUser Tests
        // ==========================================

        [Fact]
        public async Task AddUser_WhenValid_PersistsUserAndPublishesEvent()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var request = new AddUserRequest
            {
                Name = "Alice",
                UidFB = userGuid.ToString(),
                Token = "token-123"
            };

            _mockUserRepo
                .Setup(r => r.AddEntity(It.Is<User>(u => u.Name == "Alice" && u.UIDFB == userGuid && u.Token == "token-123")))
                .Returns(true);

            // Act
            var reply = await _service.AddUser(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockUserRepo.Verify(r => r.AddEntity(It.IsAny<User>()), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<AddUserEvent>(e =>
                e.Name == "Alice" &&
                e.Guid == userGuid.ToString() &&
                e.Token == "token-123"
            )), Times.Once);
        }

        [Fact]
        public async Task AddUser_WhenPersistFails_DoesNotPublishEvent()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var request = new AddUserRequest
            {
                Name = "Bob",
                UidFB = userGuid.ToString(),
                Token = "token-456"
            };

            _mockUserRepo
                .Setup(r => r.AddEntity(It.IsAny<User>()))
                .Returns(false);

            // Act
            var reply = await _service.AddUser(request, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeFalse();

            _mockUserRepo.Verify(r => r.AddEntity(It.IsAny<User>()), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddUserEvent>()), Times.Never);
        }

        [Fact]
        public async Task AddUser_WhenUidFBInvalidFormat_ThrowsFormatException()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Invalid",
                UidFB = "not-a-guid",
                Token = "token"
            };

            // Act
            Func<Task> act = async () => await _service.AddUser(request, _context);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockUserRepo.Verify(r => r.AddEntity(It.IsAny<User>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddUserEvent>()), Times.Never);
        }

        // ==========================================
        // 2. GetUsers Tests
        // ==========================================

        [Fact]
        public async Task GetUsers_WhenUsersExist_ReturnsNamesAndUids()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var users = new List<User>
            {
                new() { Name = "Alice", UIDFB = guid1 },
                new() { Name = "Bob", UIDFB = guid2 }
            };

            _mockUserRepo.Setup(r => r.GetEntities()).Returns(users);

            // Act
            var reply = await _service.GetUsers(new GetUsersRequest(), _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Names.Should().ContainInOrder("Alice", "Bob");
            reply.Uids.Should().ContainInOrder(guid1.ToString(), guid2.ToString());
        }

        [Fact]
        public async Task GetUsers_WhenNoUsers_ReturnsEmptyReply()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.GetEntities()).Returns(new List<User>());

            // Act
            var reply = await _service.GetUsers(new GetUsersRequest(), _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Names.Should().BeEmpty();
            reply.Uids.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUsers_WhenRepositoryReturnsNull_ReturnsEmptyReplySafely()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.GetEntities()).Returns((List<User>)null!);

            // Act
            var reply = await _service.GetUsers(new GetUsersRequest(), _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Names.Should().BeEmpty();
            reply.Uids.Should().BeEmpty();
        }

        // ==========================================
        // 3. GetUser Tests
        // ==========================================

        [Fact]
        public async Task GetUser_WhenUserFound_ReturnsPopulatedReply()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var user = new User { Name = "Charlie", UIDFB = guid };

            _mockUserRepo.Setup(r => r.GetUserByUIDFB(guid)).Returns(user);

            // Act
            var reply = await _service.GetUser(new GetUserRequest { UidFB = guid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Name.Should().Be("Charlie");
            reply.UidFB.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task GetUser_WhenUserNotFound_ReturnsEmptyReply()
        {
            // Arrange
            var guid = Guid.NewGuid();
            _mockUserRepo.Setup(r => r.GetUserByUIDFB(guid)).Returns((User)null!);

            // Act
            var reply = await _service.GetUser(new GetUserRequest { UidFB = guid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            reply.Name.Should().BeEmpty();
            reply.UidFB.Should().BeEmpty();
        }

        // ==========================================
        // 4. DelUser Tests
        // ==========================================

        [Fact]
        public async Task DelUser_WhenUserFoundAndDeleted_PublishesDelUserEvent()
        {
            // Arrange
            var userUid = Guid.NewGuid();
            var userEntityGuid = Guid.NewGuid();
            var user = new User
            {
                Guid = userEntityGuid,
                Name = "David",
                UIDFB = userUid,
                Token = "token-david"
            };

            _mockUserRepo.Setup(r => r.GetUserByUIDFB(userUid)).Returns(user);
            _mockUserRepo.Setup(r => r.DelEntity(userEntityGuid)).Returns(true);

            // Act
            var reply = await _service.DelUser(new DelUserRequest { UidFB = userUid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            _mockUserRepo.Verify(r => r.DelEntity(userEntityGuid), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<DelUserEvent>(e =>
                e.Name == "David" &&
                e.Guid == userUid.ToString() &&
                e.Token == "token-david"
            )), Times.Once);
        }

        [Fact]
        public async Task DelUser_WhenUserNotFound_DoesNotDeleteOrPublish()
        {
            // Arrange
            var userUid = Guid.NewGuid();
            _mockUserRepo.Setup(r => r.GetUserByUIDFB(userUid)).Returns((User)null!);

            // Act
            var reply = await _service.DelUser(new DelUserRequest { UidFB = userUid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            _mockUserRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelUserEvent>()), Times.Never);
        }

        [Fact]
        public async Task DelUser_WhenDelEntityFails_DoesNotPublishEvent()
        {
            // Arrange
            var userUid = Guid.NewGuid();
            var userEntityGuid = Guid.NewGuid();
            var user = new User
            {
                Guid = userEntityGuid,
                Name = "Eve",
                UIDFB = userUid,
                Token = "token-eve"
            };

            _mockUserRepo.Setup(r => r.GetUserByUIDFB(userUid)).Returns(user);
            _mockUserRepo.Setup(r => r.DelEntity(userEntityGuid)).Returns(false);

            // Act
            var reply = await _service.DelUser(new DelUserRequest { UidFB = userUid.ToString() }, _context);

            // Assert
            reply.Should().NotBeNull();
            _mockUserRepo.Verify(r => r.DelEntity(userEntityGuid), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelUserEvent>()), Times.Never);
        }

        // ==========================================
        // 5. UpdateUser Test
        // ==========================================

        [Fact]
        public async Task UpdateUser_CallsBaseMethod_ThrowsRpcException()
        {
            // Act
            Func<Task> act = async () => await _service.UpdateUser(new UpdateUserRequest(), _context);

            // Assert
            var ex = await act.Should().ThrowAsync<RpcException>();
            ex.Which.StatusCode.Should().Be(StatusCode.Unimplemented);
        }
    }
}
