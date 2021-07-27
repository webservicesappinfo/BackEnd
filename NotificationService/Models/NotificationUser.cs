using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Models
{
    public class NotificationUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Token { get; set; }
        public string LastSendMessage { get; set; }
        public string LastGetMessage { get; set; }
    }
}
