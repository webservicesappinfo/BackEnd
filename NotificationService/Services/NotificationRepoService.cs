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
        private readonly IMobileMessagingService _messaging;

        public NotificationRepoService(ILogger<NotificationRepoService> logger, IMobileMessagingService messaging) : base(logger) 
        { 
            _logger = logger; 
            _messaging = messaging;
        }

        public NotificationUser GetUserByUIDFB(Guid uidfb)
        {
            using (var db = new NotificationContext())
                return db.Values.FirstOrDefault(x => x.UIDFB == uidfb);
        }

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

        public bool SendNotification(Guid fromUserGuid, Guid ForUserGuid, string msg)
        {
            var forUser = GetUserByUIDFB(ForUserGuid);
            var fromUser = GetUserByUIDFB(fromUserGuid);
            if (forUser == null || fromUser == null) return false;
            _messaging.SendNotification(forUser.Token, "NewMsg", msg);
            return true;
        }
    }
}
