using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OrderService.Abstractions;
using OrderService.Protos;
using System;
using System.Linq;
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
            var order = new Models.Order()
            {
                Name = request.Name,
                OfferGuid = new Guid(request.OfferGuid),
                UserGuid = new Guid(request.UserGuid),
                UserName = request.UserName,
                MasterGuid = new Guid(request.MasterGuid),
                MasterName = request.MasterName,
                SkillGuid = new Guid(request.SkillGuid),
                SkillName = request.SkillName
            };
            var result = _orderRepoService.AddEntity(order);
            if (result)
                _eventBus.Publish(new AddOrderEvent(order.Name, order.Guid, order.OfferGuid));
            return Task.FromResult(new AddOrderReply { Result = result });
        }

        public override Task<GetOrderReply> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            return base.GetOrder(request, context);
        }

        public override Task<GetOrdersReply> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            var userGuid = new Guid(request.UserGuid);
            var orders = request.IsMaster ? _orderRepoService.GetEntities().Where(x => x.MasterGuid == userGuid)
                : _orderRepoService.GetEntities().Where(x => x.UserGuid == userGuid);

            var reply = new GetOrdersReply();

            foreach (var order in orders)
            {
                reply.Guids.Add(order.Guid.ToString());
                reply.Names.Add(order.Name);
                reply.UserNames.Add(order.UserName);
                reply.UserGuids.Add(order.UserGuid.ToString());
                reply.MasterNames.Add(order.MasterName);
                reply.MasterGuids.Add(order.MasterGuid.ToString());
                reply.SkillNames.Add(order.SkillName);
                reply.SkillGuids.Add(order.SkillGuid.ToString());
                reply.Statuses.Add(order.Status.ToString());
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
