using Globals.Abstractions;
using OrderService.Models;

namespace OrderService.Abstractions
{
    public interface IOrderRepoService : IRepoServiceBase<Order> {}
}
