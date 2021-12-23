using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using System;
using System.Threading.Tasks;

namespace OfferService.EventHandlers
{
    public class AddOrderEH : IIntegrationEventHandler<AddOrderEvent>
    {
        private readonly ILogger<AddOrderEvent> _logger;
        private readonly IOfferRepoService _offerRepoService;

        public AddOrderEH(ILogger<AddOrderEvent> logger, IOfferRepoService userRepoService)
        {
            _logger = logger;
            _offerRepoService = userRepoService;
        }

        public Task Handle(AddOrderEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            var result = _offerRepoService.SetStatus(@event.OfferGuid, Models.OfferStatus.Submitted);
            return Task.FromResult(result);
        }
    }
}
