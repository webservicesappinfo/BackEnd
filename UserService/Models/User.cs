using Globals.Models;
using Microsoft.EntityFrameworkCore;
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
        public List<CompanyRef> Companies { get; } = new List<CompanyRef>();
        public List<OfferRef> Offers { get; } = new List<OfferRef>();
    }

    public class UserContext : ContextBase<User> 
    {
        public DbSet<CompanyRef> Companies { get; set; }
        public DbSet<OfferRef> Offers { get; set; }
    }
}
