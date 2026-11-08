using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.SkillEvents;
using FluentAssertions;
using Globals.Tests;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using SkillService.Abstractions;
using SkillService.Protos;
using SkillService.Services;
using Xunit;
using SkillEntity = SkillService.Models.Skill;

namespace SkillService.Tests.Services
{
    public class SkillServiceImpTests
    {
        private readonly Mock<ILogger<SkillServiceImp>> _mockLogger;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<ISkillRepoService> _mockRepo;
        private readonly TestServerCallContext _callContext;
        private readonly SkillServiceImp _service;

        public SkillServiceImpTests()
        {
            _mockLogger = new Mock<ILogger<SkillServiceImp>>();
            _mockEventBus = new Mock<IEventBus>();
            _mockRepo = new Mock<ISkillRepoService>();
            _callContext = new TestServerCallContext();

            _service = new SkillServiceImp(
                _mockLogger.Object,
                _mockEventBus.Object,
                _mockRepo.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenValidArguments_InitializesCorrectly()
        {
            // Act
            var service = new SkillServiceImp(
                _mockLogger.Object,
                _mockEventBus.Object,
                _mockRepo.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region AddSkill Tests

        [Fact]
        public async Task AddSkill_WhenValid_PersistsEntityAndPublishesAddSkillEvent()
        {
            // Arrange
            var generatedSkillGuid = Guid.NewGuid();
            var request = new AddSkillRequest
            {
                Name = "Hair Styling",
                Desc = "Professional hair styling and grooming"
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<SkillEntity>()))
                .Callback<SkillEntity>(s => s.Guid = generatedSkillGuid)
                .Returns(true);

            // Act
            var reply = await _service.AddSkill(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.AddEntity(It.Is<SkillEntity>(s =>
                s.Name == "Hair Styling" &&
                s.Description == "Professional hair styling and grooming")), Times.Once);

            _mockEventBus.Verify(b => b.Publish(It.Is<AddSkillEvent>(e =>
                e.Name == "Hair Styling" &&
                e.Guid == generatedSkillGuid)), Times.Once);
        }

        [Fact]
        public async Task AddSkill_WhenRepositoryFails_DoesNotPublishEventAndReturnsFalse()
        {
            // Arrange
            var request = new AddSkillRequest
            {
                Name = "Manicure",
                Desc = "Nail care service"
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<SkillEntity>()))
                .Returns(false);

            // Act
            var reply = await _service.AddSkill(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeFalse();

            _mockRepo.Verify(r => r.AddEntity(It.IsAny<SkillEntity>()), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddSkillEvent>()), Times.Never);
        }

        [Fact]
        public async Task AddSkill_WhenFieldsAreEmpty_PersistsEntityAndPublishesEvent()
        {
            // Arrange
            var generatedSkillGuid = Guid.NewGuid();
            var request = new AddSkillRequest
            {
                Name = string.Empty,
                Desc = string.Empty
            };

            _mockRepo.Setup(r => r.AddEntity(It.IsAny<SkillEntity>()))
                .Callback<SkillEntity>(s => s.Guid = generatedSkillGuid)
                .Returns(true);

            // Act
            var reply = await _service.AddSkill(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.AddEntity(It.Is<SkillEntity>(s =>
                s.Name == string.Empty &&
                s.Description == string.Empty)), Times.Once);

            _mockEventBus.Verify(b => b.Publish(It.Is<AddSkillEvent>(e =>
                e.Name == string.Empty &&
                e.Guid == generatedSkillGuid)), Times.Once);
        }

        [Fact]
        public async Task AddSkill_WhenRequestIsNull_ThrowsNullReferenceException()
        {
            // Act
            var act = async () => await _service.AddSkill(null!, _callContext);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
            _mockRepo.Verify(r => r.AddEntity(It.IsAny<SkillEntity>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<AddSkillEvent>()), Times.Never);
        }

        #endregion

        #region GetSkills Tests

        [Fact]
        public async Task GetSkills_WhenSkillsExist_ReturnsPopulatedReply()
        {
            // Arrange
            var skill1Guid = Guid.NewGuid();
            var skill2Guid = Guid.NewGuid();
            var skill3Guid = Guid.NewGuid();

            var skills = new List<SkillEntity>
            {
                new() { Guid = skill1Guid, Name = "Haircut", Description = "Men and women haircut" },
                new() { Guid = skill2Guid, Name = "Coloring", Description = "Hair coloring and highlights" },
                new() { Guid = skill3Guid, Name = "Beard Trim", Description = "Beard grooming" }
            };

            _mockRepo.Setup(r => r.GetEntities()).Returns(skills);

            var request = new GetSkillsRequest();

            // Act
            var reply = await _service.GetSkills(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Guids.Should().HaveCount(3);
            reply.Names.Should().HaveCount(3);
            reply.Desc.Should().HaveCount(3);

            reply.Guids.Should().ContainInOrder(skill1Guid.ToString(), skill2Guid.ToString(), skill3Guid.ToString());
            reply.Names.Should().ContainInOrder("Haircut", "Coloring", "Beard Trim");
            reply.Desc.Should().ContainInOrder("Men and women haircut", "Hair coloring and highlights", "Beard grooming");

            _mockRepo.Verify(r => r.GetEntities(), Times.Once);
        }

        [Fact]
        public async Task GetSkills_WhenNoSkillsExist_ReturnsEmptyReply()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetEntities()).Returns(new List<SkillEntity>());

            var request = new GetSkillsRequest();

            // Act
            var reply = await _service.GetSkills(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Guids.Should().BeEmpty();
            reply.Names.Should().BeEmpty();
            reply.Desc.Should().BeEmpty();

            _mockRepo.Verify(r => r.GetEntities(), Times.Once);
        }

        [Fact]
        public async Task GetSkills_WhenUserGuidProvided_ReturnsSkillsFromRepository()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var skillGuid = Guid.NewGuid();
            var skills = new List<SkillEntity>
            {
                new() { Guid = skillGuid, Name = "Massage", Description = "Relaxation massage" }
            };

            _mockRepo.Setup(r => r.GetEntities()).Returns(skills);

            var request = new GetSkillsRequest { UserGuid = userGuid.ToString() };

            // Act
            var reply = await _service.GetSkills(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Guids.Should().ContainSingle().Which.Should().Be(skillGuid.ToString());
            reply.Names.Should().ContainSingle().Which.Should().Be("Massage");
            reply.Desc.Should().ContainSingle().Which.Should().Be("Relaxation massage");

            _mockRepo.Verify(r => r.GetEntities(), Times.Once);
        }

        [Fact]
        public async Task GetSkills_WhenSingleSkillExists_MapsAllFieldsAccurately()
        {
            // Arrange
            var skillGuid = Guid.NewGuid();
            var skills = new List<SkillEntity>
            {
                new() { Guid = skillGuid, Name = "Makeup", Description = "Evening makeup" }
            };

            _mockRepo.Setup(r => r.GetEntities()).Returns(skills);

            // Act
            var reply = await _service.GetSkills(new GetSkillsRequest(), _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Guids.Should().ContainSingle().Which.Should().Be(skillGuid.ToString());
            reply.Names.Should().ContainSingle().Which.Should().Be("Makeup");
            reply.Desc.Should().ContainSingle().Which.Should().Be("Evening makeup");
        }

        #endregion

        #region GetSkill Tests

        [Fact]
        public async Task GetSkill_WhenCalled_CallsBaseImplementationAndThrowsRpcExceptionUnimplemented()
        {
            // Arrange
            var request = new GetSkillRequest { Guid = Guid.NewGuid().ToString() };

            // Act
            var act = async () => await _service.GetSkill(request, _callContext);

            // Assert
            var exception = await act.Should().ThrowAsync<RpcException>();
            exception.Which.StatusCode.Should().Be(StatusCode.Unimplemented);
        }

        #endregion

        #region UpdateSkill Tests

        [Fact]
        public async Task UpdateSkill_WhenCalled_CallsBaseImplementationAndThrowsRpcExceptionUnimplemented()
        {
            // Arrange
            var request = new UpdateSkillRequest
            {
                Guid = Guid.NewGuid().ToString(),
                Name = "Updated Name"
            };

            // Act
            var act = async () => await _service.UpdateSkill(request, _callContext);

            // Assert
            var exception = await act.Should().ThrowAsync<RpcException>();
            exception.Which.StatusCode.Should().Be(StatusCode.Unimplemented);
        }

        #endregion

        #region DelSkill Tests

        [Fact]
        public async Task DelSkill_WhenValid_DeletesEntityAndPublishesDelSkillEvent()
        {
            // Arrange
            var skillGuid = Guid.NewGuid();
            var request = new DelSkillRequest
            {
                Guid = skillGuid.ToString()
            };

            _mockRepo.Setup(r => r.DelEntity(skillGuid)).Returns(true);

            // Act
            var reply = await _service.DelSkill(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeTrue();

            _mockRepo.Verify(r => r.DelEntity(skillGuid), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.Is<DelSkillEvent>(e =>
                e.Guid == skillGuid)), Times.Once);
        }

        [Fact]
        public async Task DelSkill_WhenRepositoryFails_DoesNotPublishEventAndReturnsFalse()
        {
            // Arrange
            var skillGuid = Guid.NewGuid();
            var request = new DelSkillRequest
            {
                Guid = skillGuid.ToString()
            };

            _mockRepo.Setup(r => r.DelEntity(skillGuid)).Returns(false);

            // Act
            var reply = await _service.DelSkill(request, _callContext);

            // Assert
            reply.Should().NotBeNull();
            reply.Result.Should().BeFalse();

            _mockRepo.Verify(r => r.DelEntity(skillGuid), Times.Once);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelSkillEvent>()), Times.Never);
        }

        [Fact]
        public async Task DelSkill_WhenGuidInvalidFormat_ThrowsFormatException()
        {
            // Arrange
            var request = new DelSkillRequest
            {
                Guid = "invalid-guid"
            };

            // Act
            var act = async () => await _service.DelSkill(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelSkillEvent>()), Times.Never);
        }

        [Fact]
        public async Task DelSkill_WhenGuidIsEmptyString_ThrowsFormatException()
        {
            // Arrange
            var request = new DelSkillRequest
            {
                Guid = string.Empty
            };

            // Act
            var act = async () => await _service.DelSkill(request, _callContext);

            // Assert
            await act.Should().ThrowAsync<FormatException>();
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelSkillEvent>()), Times.Never);
        }

        [Fact]
        public async Task DelSkill_WhenRequestIsNull_ThrowsNullReferenceException()
        {
            // Act
            var act = async () => await _service.DelSkill(null!, _callContext);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
            _mockRepo.Verify(r => r.DelEntity(It.IsAny<Guid>()), Times.Never);
            _mockEventBus.Verify(b => b.Publish(It.IsAny<DelSkillEvent>()), Times.Never);
        }

        [Fact]
        public void DelSkillRequest_WhenGuidSetToNull_ThrowsArgumentNullException()
        {
            // Act
            var act = () => new DelSkillRequest { Guid = null! };

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        #endregion
    }
}
