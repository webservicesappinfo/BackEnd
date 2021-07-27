using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotificationRepoService : INotificationRepoService
    {
        private readonly ILogger<NotificationRepoService> _logger;
        private readonly IEventBus _eventBus;

        public NotificationRepoService(ILogger<NotificationRepoService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public NotificationUser GetUser(string guid)
        {
            NotificationUser user = null;
            using (var db = new NotificationContext())
                user = db.Users.FirstOrDefault(x => x.Guid == guid);
            return user;
        }

        public Boolean AddUser(NotificationUser user)
        {
            using (var db = new NotificationContext())
            {
                if (db.Users.Any(x => x.Guid == user.Guid)) return false;
                db.Users.Add(user);
                db.SaveChanges();
            }
            return true;
        }

        public List<NotificationUser> GetUsers()
        {
            var users = new List<NotificationUser>();
            using (var db = new NotificationContext())
                users = db.Users.ToList();
            return users;
        }

        public string GetLastGetMessage(string userGuid)
        {
            using (var db = new NotificationContext())
                return db.Users.FirstOrDefault(x => x.Guid == userGuid)?.LastGetMessage ?? String.Empty;
        }

        public void SetLastGetMessage(string userGuid, string msg)
        {
            using (var db = new NotificationContext())
            {
                var findUser = db.Users.FirstOrDefault(x => x.Guid == userGuid);
                if (findUser != null) findUser.LastGetMessage = msg;
            }
        }

        public string GetLastSendMessage(string userGuid)
        {
            using (var db = new NotificationContext())
                return db.Users.FirstOrDefault(x => x.Guid == userGuid)?.LastSendMessage ?? String.Empty;
        }

        public void SetLastSendMessage(string userGuid, string msg)
        {
            using (var db = new NotificationContext())
            {
                var findUser = db.Users.FirstOrDefault(x => x.Guid == userGuid);
                if (findUser != null) findUser.LastSendMessage = msg;
            }
        }
    }
}
