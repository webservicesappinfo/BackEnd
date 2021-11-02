using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public String Guid { get; set; }
        public String CompanyGuid { get; set; }
        public String SkillGuid { get; set; }
        public String Desc { get; set; }
        public Double Price { get; set; }

    }
}
