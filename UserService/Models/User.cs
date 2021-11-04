using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class User:EntityBase
    {
        public string Token { get; set; }
        public String Name { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public List<CompanyRef> Companies { get; set; }
        public List<OfferRef> Offers { get; set; }
    }

    public class UserContext : ContextBase<User> { }
}
