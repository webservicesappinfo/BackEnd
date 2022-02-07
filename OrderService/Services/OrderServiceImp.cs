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
                _eventBus.Publish(new AddOrderEvent(order.Name, order.Guid, order.OfferGuid, order.MasterGuid, order.UserGuid, order.UserName));
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
                reply.OfferGuids.Add(order.OfferGuid.ToString());
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
            var orderGuid = new Guid(request.Guid);
            var delOrder = _orderRepoService.GetEntity(orderGuid);
            var result = _orderRepoService.DelEntity(orderGuid);
            if (delOrder != null && result)
                _eventBus.Publish(new DelOrderEvent(delOrder.Name, delOrder.Guid, delOrder.OfferGuid));
            return Task.FromResult(new DelOrderReply { Result = result });
        }

        public override Task<AcceptedOrderReply> AcceptedOrder(AcceptedOrderRequest request, ServerCallContext context)
        {
            var order = _orderRepoService.SetOrderStatus(new Guid(request.Guid), Models.OrderStatus.Accepted);
            if (order != null)
                _eventBus.Publish(new AcceptedOrderEvent(order.Name, order.Guid, order.MasterGuid, order.MasterName, order.UserGuid, order.UserName));
            return Task.FromResult(new AcceptedOrderReply { Result = order != null, Name = order.Name, ClientGuid = order?.UserGuid.ToString(), MasterGuid = order?.MasterGuid.ToString() });
        }

        public override Task<ExecutedOrderReply> ExecutedOrder(ExecutedOrderRequest request, ServerCallContext context)
        {
            var order = _orderRepoService.SetOrderStatus(new Guid(request.Guid), Models.OrderStatus.Executed);
            if(order != null)
                _eventBus.Publish(new ExecutedOrderEvent(order.Name, order.Guid, order.MasterGuid, order.MasterName, order.UserGuid, order.UserName));
            return Task.FromResult(new ExecutedOrderReply { Result =  order != null });
        }
    }
}
