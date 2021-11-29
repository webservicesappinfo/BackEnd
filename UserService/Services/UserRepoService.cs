using EventBus.Abstractions;
using Microsoft.EntityFrameworkCore;
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

        public bool AddUser(User user)
        {
            using (var db = new UserContext())
            {
                if (!db.Values.Any(x => x.UIDFB == user.UIDFB))
                {
                    db.Values.Add(user);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public User GetUser(Guid guid)
        {
            User user = null;
            using (var db = new UserContext())
                user = db.Values.Include(x => x.Companies).Include(x => x.Offers).FirstOrDefault(x => x.Guid == guid);
            return user;
        }

        public List<User> GetAllUsers()
        {
            using (var db = new UserContext())
            {
                var users = db.Values.ToList();
                return users;
                //return db.Values.Include(x => x.Companies).Include(x => x.Offers).ToList();
            }
        }

        public Boolean UpdateUser(User user)
        {
            return false;
        }

        public Boolean DelUser(User user)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.FirstOrDefault(x => x.UIDFB == user.UIDFB);

                if (findUser == null) return false;
                db.Remove(findUser);
                db.SaveChanges();

                /*var findUser = db.Values.Include(x=>x.Companies).FirstOrDefault(x => x.UIDFB == user.UIDFB);
                if (findUser == null) return false;

                foreach (var c in findUser.Companies)
                    db.Companies.Remove(c);
                db.SaveChanges();

                foreach (var o in findUser.Offers)
                    db.Offers.Remove(o);
                db.SaveChanges();

                db.Remove(findUser);
                db.SaveChanges();*/
            }
            return true;
        }

        public bool AddCompany(Guid uidfb, Guid company)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.FirstOrDefault(x => x.UIDFB == uidfb);
                if (findUser == null) return false;
                if (findUser.Companies.Any(x => x.Guid == company)) return false;
                findUser.Companies.Add(new Globals.Models.CompanyRef<User>() { RefGuid = company });
                db.SaveChanges();
            }
            return true;
        }

        public bool DelCompany(Guid uidfb, Guid company)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.FirstOrDefault(x => x.UIDFB == uidfb);
                if (findUser == null) return false;
                var findCompany = findUser.Companies.FirstOrDefault(x => x.Guid == company);
                if (findCompany == null) return false;
                findUser.Companies.Remove(findCompany);
                db.SaveChanges();
            }
            return true;
        }

        public User GetUserByUIDFB(Guid uidfb)
        {
            User user = null;
            using (var db = new UserContext())
                user = db.Values.FirstOrDefault(x => x.UIDFB == uidfb);
            return user;
        }
    }
}
