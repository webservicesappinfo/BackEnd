using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Abstractions
{
    public interface IUserRepoService
    {
        public Boolean AddUser(String uidFB, String name);

        public User GetUser(Guid guid);        

        public List<User> GetAllUsers();

        public Boolean UpdateUser(User user);

        public Boolean DelUser(User user);

        public Boolean AddCompany(Guid user, Guid company);
    }
}
