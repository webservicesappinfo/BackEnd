using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using EventBus.Events.ServicesEvents.OrderEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MobileApiGetway;
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
                Name = request.Order.Name,
                MasterGuid = new Guid(request.Order.MasterGuid),
                MasterName = request.Order.MasterName,
                SkillGuid = new Guid(request.Order.SkillGuid),
                SkillName = request.Order.SkillName
            };
            var result = _orderRepoService.AddEntity(order);

            if (result)
                _eventBus.Publish(new AddOrderEvent(order.Name, order.Guid, order.OfferGuid, order.MasterGuid, order.UserGuid, order.UserName));

            return Task.FromResult(new AddOrderReply { Result = result });
        }
        public override Task<DelOrderReply> DelOrder(DelOrderRequest request, ServerCallContext context)
        {
            var order = request.Order;
            var result = _orderRepoService.DelEntity(new Guid(order.Guid));
            if (result)
                _eventBus.Publish(new DelOrderEvent(order.Name, new Guid(order.Guid), new Guid(order.OfferGuid)));
            return Task.FromResult(new DelOrderReply() { Result = result });
        }
    }
}
