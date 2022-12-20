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
        public List<UserCompanyRef> OwnCompanies { get; } = new List<UserCompanyRef>();
        public List<MasterCompanyRef> MasterCompanies { get; } = new List<MasterCompanyRef>();
        public List<UserOfferRef> Offers { get; } = new List<UserOfferRef>();
    }

    public class UserContext : ContextBase<User>
    {
        public DbSet<UserCompanyRef> OwnCompanies { get; set; }
        public DbSet<MasterCompanyRef> MasterCompanies { get; set; }
        public DbSet<UserOfferRef> Offers { get; set; }

        protected override void ModelBuilderConfigure(ModelBuilder builder)
        {
            builder.Entity<UserCompanyRef>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.OwnCompanies)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MasterCompanyRef>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.MasterCompanies)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserOfferRef>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Offers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
