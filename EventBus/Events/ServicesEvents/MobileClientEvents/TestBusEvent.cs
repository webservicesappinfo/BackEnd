using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.ServicesEvents.MobileClientEvents
{
    public class TestBusEvent: IntegrationEvent
    {
        public String Msg { get; set; }
    }
}
