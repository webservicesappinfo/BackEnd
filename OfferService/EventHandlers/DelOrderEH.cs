using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using System;
using System.Threading.Tasks;

namespace OfferService.EventHandlers
{
    public class DelOrderEH : IIntegrationEventHandler<DelOrderEvent>
    {
        private readonly ILogger<DelOrderEvent> _logger;
        private readonly IOfferRepoService _offerRepoService;

        public DelOrderEH(ILogger<DelOrderEvent> logger, IOfferRepoService userRepoService)
        {
            _logger = logger;
            _offerRepoService = userRepoService;
        }

        public Task Handle(DelOrderEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            var offer = _offerRepoService.GetEntity(@event.OfferGuid);
            if (offer.Status == Models.OfferStatus.Submitted || offer.Status == Models.OfferStatus.Accepted)
            {
                var result = _offerRepoService.SetStatus(@event.OfferGuid, Models.OfferStatus.Actived);
                return Task.FromResult(result);
            }
            return Task.FromResult(false);
        }
    }
}
