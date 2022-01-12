using Globals.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyService.Models
{
    public class Company : EntityBase
    {
        public string Name { get; set; }
        public Guid OwnerGuid { get; set; }
        public String OwnerName { get; set; }
        public List<MasterRef<Company>> Masters { get; } = new List<MasterRef<Company>>();
        public List<OfferRef<Company>> Offers { get; } = new List<OfferRef<Company>>(); 
    }

    public class CompanyContext : ContextBase<Company> 
    {
        public DbSet<MasterRef<Company>> Masters{ get; set; }
        public DbSet<OfferRef<Company>> Offers { get; set; }

        protected override void ModelBuilderConfigure(ModelBuilder builder)
        {
            builder.Entity<MasterRef<Company>>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Masters)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OfferRef<Company>>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Offers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
