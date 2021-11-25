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
        public Guid User { get; set; }
        public List<MasterRef> Masters { get; } = new List<MasterRef>();
        public List<OfferRef> Offers { get; } = new List<OfferRef>(); 
    }

    public class CompanyContext : ContextBase<Company> 
    {
        public DbSet<MasterRef> Masters{ get; set; }
        public DbSet<OfferRef> Offers { get; set; }
    }
}
