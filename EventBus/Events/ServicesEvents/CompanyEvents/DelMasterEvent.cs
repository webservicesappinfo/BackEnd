using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.CompanyEvents
{
    public class DelMasterEvent : IntegrationEvent
    {
        public Guid CompanyGuid { get; private set; }
        public String CompanyName { get; set; }
        public Guid MasterUIDFB { get; private set; }

        public DelMasterEvent(Guid companyGuid, Guid masterUIDFB, String companyName)
        {
            CompanyGuid = companyGuid;
            MasterUIDFB = masterUIDFB;
            CompanyName = companyName;
        }
    }
}
