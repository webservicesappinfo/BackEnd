using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order : EntityBase
    {
        public String Name { get; set; }
        public Guid OfferGuid { get; set; }
        public Guid UserGuid { get; set; }
        public String UserName { get; set; }
        public Guid MasterGuid { get; set; }
        public String MasterName { get; set; }
        public Guid SkillGuid { get; set; }
        public String SkillName { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class OrderContext : ContextBase<Order> { }

    public enum OrderStatus
    {
        Submitted,
        Accepted,
        Executed,
        UnExecuted,
        Canceled
    }
}
