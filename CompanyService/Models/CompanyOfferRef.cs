using Globals.Models;

namespace CompanyService.Models
{
    public class CompanyOfferRef : RefBase<CompanyUserRef>
    {
        public string Name { get; set; }
    }
}
