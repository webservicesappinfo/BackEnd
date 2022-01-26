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
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public List<Worker> Masters { get; } = new List<Worker>();
        public List<OfferRef<Company>> Offers { get; } = new List<OfferRef<Company>>(); 
    }

    public class CompanyContext : ContextBase<Company> 
    {
        public DbSet<Worker> Masters{ get; set; }
        public DbSet<OfferRef<Company>> Offers { get; set; }

        protected override void ModelBuilderConfigure(ModelBuilder builder)
        {
            builder.Entity<Worker>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Masters)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OfferRef<Company>>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Offers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class Worker: UserRef<Company>
    {

    }
}
