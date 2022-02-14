using Globals.Sevices;
using Microsoft.Extensions.Logging;
using OrderService.Abstractions;
using OrderService.Models;
using System;
using System.Linq;

namespace OrderService.Services
{
    public class OrderRepoService : RepoServiceBase<Order, OrderContext>, IOrderRepoService
    {
        public OrderRepoService(ILogger<RepoServiceBase<Order, OrderContext>> logger) : base(logger) { }

        public bool OnDelOfferEH(Guid offerGuid, Guid masterGuid)
        {
            var orders = GetEntities().Where(x=>x.OfferGuid == offerGuid && x.MasterGuid == masterGuid).ToList();
            foreach (var order in orders)
                DelEntity(order.Guid);
            return true;
        }

        public Order SetOrderStatus(Guid orderGuid, OrderStatus status)
        {
            Order order = null;
            using (var db = new OrderContext())
            {
                order = db.Values.FirstOrDefault(x => x.Guid == orderGuid);
                if (order == null) return order;
                order.Status = status;
                db.SaveChanges();
            }
            return order;
        }
    }
}
