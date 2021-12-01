using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.SkillEvents
{
    public class DelSkillEvent : IntegrationEvent
    {
            public Guid Guid { get; private set; }

            public DelSkillEvent(Guid guid) => Guid = guid;
    }
}
