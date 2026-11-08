using System;
using EventBus.Events;
using EventBus.Events.ServicesEvents.SkillEvents;
using FluentAssertions;
using Xunit;

namespace SkillService.Tests.Events
{
    public class SkillEventsTests
    {
        [Fact]
        public void AddSkillEvent_Constructor_SetsNameAndGuidCorrectly()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var name = "Frontend React";

            // Act
            var @event = new AddSkillEvent(name, guid);

            // Assert
            @event.Should().NotBeNull();
            @event.Should().BeAssignableTo<IntegrationEvent>();
            @event.Name.Should().Be(name);
            @event.Guid.Should().Be(guid);
            @event.Id.Should().NotBeEmpty();
            @event.CreationDate.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        }

        [Fact]
        public void DelSkillEvent_Constructor_SetsGuidCorrectly()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var @event = new DelSkillEvent(guid);

            // Assert
            @event.Should().NotBeNull();
            @event.Should().BeAssignableTo<IntegrationEvent>();
            @event.Guid.Should().Be(guid);
            @event.Id.Should().NotBeEmpty();
            @event.CreationDate.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        }
    }
}
