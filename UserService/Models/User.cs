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
        public List<CompanyRef<User>> Companies { get; } = new List<CompanyRef<User>>();
        public List<OfferRef<User>> Offers { get; } = new List<OfferRef<User>>();
    }

    public class UserContext : ContextBase<User>
    {
        public DbSet<CompanyRef<User>> Companies { get; set; }
        public DbSet<OfferRef<User>> Offers { get; set; }

        protected override void ModelBuilderConfigure(ModelBuilder builder)
        {
            builder.Entity<CompanyRef<User>>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Companies)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OfferRef<User>>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Offers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
