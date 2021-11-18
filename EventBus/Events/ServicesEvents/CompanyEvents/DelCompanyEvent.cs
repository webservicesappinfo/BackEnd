using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.CompanyEvents
{
    public class DelCompanyEvent : IntegrationEvent
    {
        public String Name { get; private set; }
        public Guid Guid { get; private set; }
        public Guid Owner { get; private set; }

        public DelCompanyEvent(String name, Guid guid, Guid owner)
        {
            Name = name;
            Guid = guid;
            Owner = owner;
        }
    }
}
