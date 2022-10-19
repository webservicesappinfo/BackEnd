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
        public List<CompanyUserRef> Workers { get; } = new List<CompanyUserRef>();
    }

    public class CompanyContext : ContextBase<Company> 
    {
        public DbSet<CompanyUserRef> Workers { get; set; }
        public DbSet<CompanyOfferRef> WorkerOffers { get; set; }

        protected override void ModelBuilderConfigure(ModelBuilder builder)
        {
            builder.Entity<CompanyUserRef>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Workers)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CompanyOfferRef>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Offers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
