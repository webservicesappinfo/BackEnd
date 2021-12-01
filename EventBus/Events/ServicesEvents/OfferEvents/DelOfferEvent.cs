using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.OfferEvents
{
    public class DelOfferEvent : IntegrationEvent
    {
        public Guid Guid { get; private set; }
        public String Name { get; private set; }
        public Guid MasterGuid { get; private set; }

        public DelOfferEvent(String name, Guid guid, Guid masterGuid)
        {
            Guid = guid;
            Name = name;
            MasterGuid = masterGuid;
        }
    }
}
