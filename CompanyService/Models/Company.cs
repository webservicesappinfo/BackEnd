using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyService.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public String OwnerGuid { get; set; }
        public List<Ref> Masters { get; set; }
        public List<Ref> Offers { get; set; }
    }
}
