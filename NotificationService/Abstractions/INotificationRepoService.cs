using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Abstractions
{
    public interface INotificationRepoService
    {
        public NotificationUser GetUser(string guid);
        public Boolean AddUser(NotificationUser user);
        public List<NotificationUser> GetUsers();
        public string GetLastGetMessage(string userGuid);
        public void SetLastGetMessage(string userGuid, string msg);
        public string GetLastSendMessage(string userGuid);
        public void SetLastSendMessage(string userGuid, string msg);
    }
}
