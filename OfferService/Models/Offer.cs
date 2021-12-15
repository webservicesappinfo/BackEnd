using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService.Models
{
    public class Offer : EntityBase
    {
        public String Name { get; set; }

        public String Description { get; set; }

        public Guid MasterGuid { get; set; }
        public String MasterName { get; set; }

        public Guid SkillGuid { get; set; }
        public String SkillName { get; set; }

    }

    public class OfferContext : ContextBase<Offer> { }
}
