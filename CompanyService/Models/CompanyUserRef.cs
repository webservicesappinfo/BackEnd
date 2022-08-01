using Globals.Models;
using System.Collections.Generic;

namespace CompanyService.Models
{
    public class CompanyUserRef : RefBase<Company>
    {
        public string Name { get; set; }
        public List<CompanyOfferRef> Offers { get; } = new List<CompanyOfferRef>();
    }
}
