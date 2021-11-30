using Globals.Sevices;
using Microsoft.Extensions.Logging;
using OrderService.Abstractions;
using OrderService.Models;

namespace OrderService.Services
{    
        public class OrderRepoService : RepoServiceBase<Order, OrderContext>, IOrderRepoService
    {
        public OrderRepoService(ILogger<RepoServiceBase<Order, OrderContext>> logger) : base(logger) { }
    }
}
