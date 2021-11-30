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

        public String UserName { get; set; }

        public String MasterName { get; set; }

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
