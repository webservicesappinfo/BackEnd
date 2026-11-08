using Autofac;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using EventBus.ServicesEvents.MobileClientEvents;
using FluentAssertions;
using LocationService.Abstractions;
using LocationService.Autofac;
using LocationService.EventHandlers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LocationService.Tests.Autofac
{
    public class EventsModuleTests
    {
        [Fact]
        public void EventsModule_Load_RegistersEventHandlersCorrectly()
        {
            // Arrange
            var builder = new ContainerBuilder();
            builder.RegisterModule(new EventsModule());

            builder.RegisterInstance(new Mock<ILogger<AddUserEH>>().Object);
            builder.RegisterInstance(new Mock<ILocationRepoService>().Object);

            // Act
            using var container = builder.Build();

            // Assert
            var addUserHandler = container.ResolveOptional<IIntegrationEventHandler<AddUserEvent>>();
            addUserHandler.Should().NotBeNull();
            addUserHandler.Should().BeOfType<AddUserEH>();

            var testBusHandler = container.ResolveOptional<IIntegrationEventHandler<TestBusEvent>>();
            testBusHandler.Should().NotBeNull();
            testBusHandler.Should().BeOfType<TestBusEventHandler>();
        }
    }
}
