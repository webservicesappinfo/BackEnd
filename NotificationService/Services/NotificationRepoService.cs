using EventBus.Abstractions;
using Globals.Sevices;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotificationRepoService : RepoServiceBase<NotificationUser, NotificationContext>, INotificationRepoService
    {
        private readonly ILogger<NotificationRepoService> _logger;
        public NotificationRepoService(ILogger<NotificationRepoService> logger) : base(logger) { _logger = logger; }

        public string GetLastGetMessage(string userGuid)
        {
            /*using (var db = new NotificationContext())
                return db.Users.FirstOrDefault(x => x.Guid == userGuid)?.LastGetMessage ?? String.Empty;*/
            return String.Empty;
        }

        public void SetLastGetMessage(string userGuid, string msg)
        {
            /*using (var db = new NotificationContext())
            {
                var findUser = db.Users.FirstOrDefault(x => x.Guid == userGuid);
                if (findUser != null) findUser.LastGetMessage = msg;
            }*/
        }

        public string GetLastSendMessage(string userGuid)
        {
            /*using (var db = new NotificationContext())
                return db.Users.FirstOrDefault(x => x.Guid == userGuid)?.LastSendMessage ?? String.Empty;*/
            return String.Empty;
        }

        public void SetLastSendMessage(string userGuid, string msg)
        {
            /*using (var db = new NotificationContext())
            {
                var findUser = db.Users.FirstOrDefault(x => x.Guid == userGuid);
                if (findUser != null) findUser.LastSendMessage = msg;
            }*/
        }
    }
}
