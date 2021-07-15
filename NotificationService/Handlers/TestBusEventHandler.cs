using EventBus.Abstractions;
using EventBus.ServicesEvents.MobileClientEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Handlers
{
    public class TestBusEventHandler : IIntegrationEventHandler<TestBusEvent>
    {
        public async Task Handle(TestBusEvent @event)
        {
            Console.WriteLine(@event.Msg);
            @event.ResponseReceivedEvent.Set();
        }
    }
}
