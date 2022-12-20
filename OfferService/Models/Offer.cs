using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService.Models
{
    public class Offer : EntityBase
    {
        private String _name;
        private String _description;
        private String _companyName;
        private String _masterName;
        private String _skillName;

        public String Name { get => _name ?? String.Empty; set => _name = value; }

        public String Description { get => _description ?? String.Empty; set => _description = value; }

        public Guid CompanyGuid { get; set; }

        public String CompanyName { get => _companyName ?? String.Empty; set => _companyName = value; }

        public Guid MasterGuid { get; set; }

        public String MasterName { get => _masterName ?? String.Empty; set => _masterName = value; }

        public Guid OrderGuid { get; set; }

        public Guid SkillGuid { get; set; }

        public String SkillName { get => _skillName ?? String.Empty; set => _skillName = value; }

        public OfferStatus Status { get; set; }

        public double? Lat { get; set; }
        public double? Lng { get; set; }

    }

    public class OfferContext : ContextBase<Offer> { }

    public enum OfferStatus
    {
        Actived,
        Submitted,
        Accepted,
        Executed,
        UnExecuted,
        Canceled
    }
}
