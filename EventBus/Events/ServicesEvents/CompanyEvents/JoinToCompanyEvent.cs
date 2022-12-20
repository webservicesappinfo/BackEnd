using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.CompanyEvents
{
    public class AddMasterEvent : IntegrationEvent
    {
        public Guid CompanyGuid { get; private set; }
        public String CompanyName { get; set; }
        public Guid MasterGuid { get; private set; }

        public AddMasterEvent(Guid companyGuid, Guid masterGuid, String companyName) 
        { 
            CompanyGuid = companyGuid; 
            MasterGuid = masterGuid; 
            CompanyName = companyName; 
        }
    }
}
