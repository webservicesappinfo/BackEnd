using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class User : EntityBase
    {
        public Guid UIDFB { get; set; }
        public string Token { get; set; }
        public String Name { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public List<CompanyRef> Companies { get; set; }
        public List<OfferRef> Offers { get; set; }

        public User()
        {
            Companies = new List<CompanyRef>();
            Offers = new List<OfferRef>();
        }
    }

    public class UserContext : ContextBase<User> {}
}
