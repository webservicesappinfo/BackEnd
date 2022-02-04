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
        public List<Worker> Workers { get; } = new List<Worker>();
    }

    public class CompanyContext : ContextBase<Company> 
    {
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkerOffer> WorkerOffers { get; set; }

        protected override void ModelBuilderConfigure(ModelBuilder builder)
        {
            builder.Entity<Worker>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.Workers)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkerOffer>()
                .HasOne(j => j.Parent)
                .WithMany(t => t.WorkerOffers)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class Worker: UserRef<Company>
    {
        public List<WorkerOffer> WorkerOffers { get; } = new List<WorkerOffer>();
    }

    public class WorkerOffer: OfferRef<Worker>
    {

    }
}
