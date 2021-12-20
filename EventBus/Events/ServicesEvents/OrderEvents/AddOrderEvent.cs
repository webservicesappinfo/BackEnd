using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.OrderEvents
{
    public class AddOrderEvent : IntegrationEvent
    {
        public Guid Guid { get; private set; }
        public String Name { get; private set; }
        public Guid OfferGuid { get; private set; }


        public AddOrderEvent(String name, Guid guid, Guid offerGuid)
        {
            Guid = guid;
            Name = name;
            OfferGuid = offerGuid;
        }
    }
}
