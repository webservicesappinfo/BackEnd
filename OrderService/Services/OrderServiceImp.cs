using EventBus.Abstractions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OrderService.Abstractions;
using OrderService.Protos;
using System;
using System.Threading.Tasks;

namespace OrderService.Services
{
    public class OrderServiceImp : Order.OrderBase
    {
        private readonly ILogger<OrderServiceImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly IOrderRepoService _orderRepoService;

        public OrderServiceImp(ILogger<OrderServiceImp> logger, IEventBus eventBus, IOrderRepoService orderRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _orderRepoService = orderRepoService;
        }

        public override Task<AddOrderReply> AddOrder(AddOrderRequest request, ServerCallContext context)
        {
            var offer = new Models.Order()
            {
                Name = request.Name,
                UserName = request.UserName,
                MasterName = request.MasterName,
                SkillName = request.SkillName
            };
            var result = _orderRepoService.AddEntity(offer);
            /*if (result)
                _eventBus.Publish(new AddSkillEvent(company.Name, company.Guid, company.User));*/
            return Task.FromResult(new AddOrderReply { Result = result });
        }

        public override Task<GetOrderReply> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            return base.GetOrder(request, context);
        }

        public override Task<GetOrdersReply> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            var orders = _orderRepoService.GetEntities();
            var reply = new GetOrdersReply();

            foreach (var order in orders)
            {
                reply.Guids.Add(order.Guid.ToString());
                reply.Names.Add(order.Name);
                reply.UserNames.Add(order.UserName);
                reply.MasterNames.Add(order.MasterName);
            }
            return Task.FromResult(reply);
        }

        public override Task<UpdateOrderReply> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {
            return base.UpdateOrder(request, context);
        }

        public override Task<DelOrderReply> DelOrder(DelOrderRequest request, ServerCallContext context)
        {
            var result = _orderRepoService.DelEntity(new Guid(request.Guid));
            return Task.FromResult(new DelOrderReply { Result = result });
        }
    }
}
