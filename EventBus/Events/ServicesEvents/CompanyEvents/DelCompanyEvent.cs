using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.CompanyEvents
{
    public class DelCompanyEvent : IntegrationEvent
    {        
        public Guid Guid { get; private set; }

        public DelCompanyEvent(Guid guid) => Guid = guid;
    }
}
