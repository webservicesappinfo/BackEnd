using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public String Guid { get; set; }
        public String CompanyrGuid { get; set; }
        public String MasterGuid { get; set; }
        public String OfferGuid { get; set; }
        public Double Price { get; set; }
    }
}
