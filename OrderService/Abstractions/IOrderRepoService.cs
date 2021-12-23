using Globals.Abstractions;
using OrderService.Models;
using System;

namespace OrderService.Abstractions
{
    public interface IOrderRepoService : IRepoServiceBase<Order> 
    {
        bool OnDelOffer(Guid offerGuid, Guid masterGuid);
    }
}
