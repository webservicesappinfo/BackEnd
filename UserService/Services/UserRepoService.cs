using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Abstractions;
using UserService.Models;

namespace UserService.Services
{
    public class UserRepoService : IUserRepoService
    {
        private readonly ILogger<UserRepoService> _logger;
        private readonly IEventBus _eventBus;

        public UserRepoService(ILogger<UserRepoService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public User GetUser(Guid guid)
        {
            User user = null;
            using (var db = new UserContext())
                user = db.Values.FirstOrDefault(x => x.Guid == guid);
            return user;
        }

        public List<User> GetAllUsers()
        {
            using (var db = new UserContext())
            return db.Values.ToList();
        }

        public Boolean UpdateUser(User user)
        {
            return false;
        }

        public Boolean DelUser(User user)
        {
            return false;
        }

        public bool AddCompany(Guid user, Guid company)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.FirstOrDefault(x => x.Guid == user);
                if (findUser == null) return false;
                if (findUser.Companies.Any(x => x.Guid == company)) return false;
                findUser.Companies.Add(new Globals.Models.CompanyRef() { RefGuid = company });
                db.SaveChanges();
            }
            return true;
        }

        public bool AddUser(String uidFB, String name)
        {
            using (var db = new UserContext())
                if (!db.Values.Any(x => x.Token == uidFB.ToString()))
                {
                    db.Values.Add(new User() { Token = uidFB, Name = name });
                    db.SaveChanges();
                    return true;
                }
            return false;
        }
    }
}
