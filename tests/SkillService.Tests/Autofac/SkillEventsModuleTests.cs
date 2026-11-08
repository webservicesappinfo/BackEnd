using Autofac;
using FluentAssertions;
using SkillService.Autofac;
using Xunit;

namespace SkillService.Tests.Autofac
{
    public class SkillEventsModuleTests
    {
        [Fact]
        public void SkillEventsModule_CanBeRegisteredInContainer()
        {
            // Arrange
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SkillEventsModule());

            // Act
            using var container = builder.Build();

            // Assert
            container.Should().NotBeNull();
        }
    }
}
