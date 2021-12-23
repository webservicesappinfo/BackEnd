using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using Microsoft.Extensions.Logging;
using OrderService.Abstractions;
using System;
using System.Threading.Tasks;

namespace OrderService.EventHandlers
{
    public class DelOfferEH : IIntegrationEventHandler<DelOfferEvent>
    {
        private readonly ILogger<DelOfferEvent> _logger;
        private readonly IOrderRepoService _orderRepoService;

        public DelOfferEH(ILogger<DelOfferEvent> logger, IOrderRepoService orderRepoService)
        {
            _logger = logger;
            _orderRepoService = orderRepoService;
        }

        public Task Handle(DelOfferEvent @event)
        {
            Console.WriteLine(@event.Guid);
            //@event.ResponseReceivedEvent.Set();
            var result = _orderRepoService.OnDelOffer(@event.Guid, @event.MasterGuid);
            return Task.FromResult(result);
        }
    }
}
