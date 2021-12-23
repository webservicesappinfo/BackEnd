using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.Extensions.Logging;
using OrderService.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.EventHandlers
{
    public class DelUserEH : IIntegrationEventHandler<DelUserEvent>
    {
        private readonly ILogger<DelUserEvent> _logger;
        private readonly IOrderRepoService _orderRepoService;

        public DelUserEH(ILogger<DelUserEvent> logger, IOrderRepoService orderRepoService)
        {
            _logger = logger;
            _orderRepoService = orderRepoService;
        }

        public Task Handle(DelUserEvent @event)
        {
            Console.WriteLine(@event.Name);
            var userGuid = new Guid(@event.Guid);
            //@event.ResponseReceivedEvent.Set();
            var offers = _orderRepoService.GetEntities().Where(x => x.MasterGuid == userGuid || x.UserGuid == userGuid);
            foreach (var offer in offers)
                _orderRepoService.DelEntity(offer.Guid);
            return Task.FromResult(true);
        }
    }
}
