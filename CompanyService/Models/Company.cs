using Globals.Models;
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
        public List<MasterRef> Masters { get; set; }
        public List<OfferRef> Offers { get; set; }
    }

    public class CompanyContext : ContextBase<Company> { }
}
