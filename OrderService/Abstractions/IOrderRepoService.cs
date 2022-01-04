using Globals.Abstractions;
using OrderService.Models;
using System;

namespace OrderService.Abstractions
{
    public interface IOrderRepoService : IRepoServiceBase<Order> 
    {
        Order SetOrderStatus(Guid orderGuid, OrderStatus status);
        bool OnDelOffer(Guid offerGuid, Guid masterGuid);
    }
}
