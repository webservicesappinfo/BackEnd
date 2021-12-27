using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Models
{
    public class NotificationUser : EntityBase
    {
        public string Name { get; set; }
        public Guid UIDFB { get; set; }
        public string Token { get; set; }
        public string LastSendMessage { get; set; }
        public string LastGetMessage { get; set; }
    }

    public class NotificationContext : ContextBase<NotificationUser> { }
}
