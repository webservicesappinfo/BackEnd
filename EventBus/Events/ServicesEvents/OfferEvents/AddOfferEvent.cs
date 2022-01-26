using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.OfferEvents
{
    public class AddOfferEvent : IntegrationEvent
    {
        public Guid Guid { get; private set; }
        public String Name { get; private set; }
        public Guid CompanyGuid { get;set; }
        public Guid MasterGuid { get; private set; }


        public AddOfferEvent(String name, Guid guid, Guid companyGuid, Guid masterGuid)
        {
            Guid = guid;
            Name = name;
            CompanyGuid = companyGuid;
            MasterGuid = masterGuid;
        }
    }
}
