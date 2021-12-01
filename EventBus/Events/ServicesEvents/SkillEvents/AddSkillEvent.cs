using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.SkillEvents
{
    public class AddSkillEvent : IntegrationEvent
    {
        public Guid Guid { get; private set; }
        public String Name { get; private set; }

        public AddSkillEvent(String name, Guid guid)
        {
            Name = name;
            Guid = guid;            
        }
    }
}
