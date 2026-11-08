using FluentAssertions;
using Globals.Sevices;
using Microsoft.Extensions.Logging;
using Moq;
using SkillService.Abstractions;
using SkillService.Models;
using SkillService.Services;
using Xunit;

namespace SkillService.Tests.Services
{
    public class SkillRepoServiceTests
    {
        [Fact]
        public void SkillRepoService_CanBeConstructed_AndImplementsISkillRepoService()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RepoServiceBase<Skill, SkillContext>>>();

            // Act
            var repoService = new SkillRepoService(mockLogger.Object);

            // Assert
            repoService.Should().NotBeNull();
            repoService.Should().BeAssignableTo<ISkillRepoService>();
        }
    }
}
