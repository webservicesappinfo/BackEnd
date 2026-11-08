using System;
using FluentAssertions;
using SkillService.Models;
using Xunit;

namespace SkillService.Tests.Models
{
    public class SkillTests
    {
        [Fact]
        public void Skill_CanSetAndGetProperties()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var name = "Software Architecture";
            var description = "Designing scalable microservices";

            // Act
            var skill = new Skill
            {
                Guid = guid,
                Name = name,
                Description = description
            };

            // Assert
            skill.Guid.Should().Be(guid);
            skill.Name.Should().Be(name);
            skill.Description.Should().Be(description);
        }

        [Fact]
        public void Skill_DefaultValues_AreExpected()
        {
            // Act
            var skill = new Skill();

            // Assert
            skill.Guid.Should().BeEmpty();
            skill.Name.Should().BeNull();
            skill.Description.Should().BeNull();
        }
    }
}
