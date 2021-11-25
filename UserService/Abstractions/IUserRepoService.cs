using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Abstractions
{
    public interface IUserRepoService
    {
        public Boolean AddUser(User user);

        public User GetUser(Guid guid);

        public List<User> GetAllUsers();

        public Boolean UpdateUser(User user);

        public Boolean DelUser(User user);

        public User GetUserByUIDFB(Guid uidfb);

        public Boolean AddCompany(Guid uidfb, Guid company);

        public Boolean DelCompany(Guid uidfb, Guid company);
    }
}
