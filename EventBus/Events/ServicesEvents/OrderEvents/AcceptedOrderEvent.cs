using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.OrderEvents
{
    public class AcceptedOrderEvent : IntegrationEvent
    {
        public Guid Guid { get; private set; }
        public String Name { get; private set; }
        public Guid MasterGuid { get; private set; }
        public String MasterName { get; private set; }
        public Guid ClientGuid { get; private set; }
        public String ClientName { get; private set; }

        public AcceptedOrderEvent(String name, Guid guid, Guid masterGuid,String masterName, Guid clientGuid, String clientName)
        {
            Guid = guid;
            Name = name;
            MasterGuid = masterGuid;
            MasterName = masterName;
            ClientGuid = clientGuid;
            ClientName = clientName;
        }
    }
}
