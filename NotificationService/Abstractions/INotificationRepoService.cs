using Globals.Abstractions;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Abstractions
{
    public interface INotificationRepoService : IRepoServiceBase<NotificationUser>
    {
        public string GetLastGetMessage(string userGuid);
        public void SetLastGetMessage(string userGuid, string msg);
        public string GetLastSendMessage(string userGuid);
        public void SetLastSendMessage(string userGuid, string msg);
    }
}
