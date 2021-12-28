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
        NotificationUser GetUserByUIDFB(Guid uidfb);
        string GetLastGetMessage(string userGuid);
        void SetLastGetMessage(string userGuid, string msg);
        string GetLastSendMessage(string userGuid);
        void SetLastSendMessage(string userGuid, string msg);
    }
}
