using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.UserRepoEvents
{
    public class AddUserEvent : IntegrationEvent
    {
        public String Name { get; private set; }
        public String Guid { get; private set; }
        public String Token { get; private set; }

        public AddUserEvent(String name, String guid, String token)
        {
            Name = name;
            Guid = guid;
            Token = token;
        }
    }
}
