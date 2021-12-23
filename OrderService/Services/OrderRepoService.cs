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

        public bool OnDelOffer(Guid offerGuid, Guid masterGuid)
        {
            var orders = GetEntities().Where(x=>x.OfferGuid == offerGuid && x.MasterGuid == masterGuid).ToList();
            foreach (var order in orders)
                DelEntity(order.Guid);
            return true;
        }
    }
}
