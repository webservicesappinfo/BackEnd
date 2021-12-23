using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService.EventHandlers
{
    public class DelUserEH : IIntegrationEventHandler<DelUserEvent>
    {
        private readonly ILogger<DelUserEvent> _logger;
        private readonly IOfferRepoService _offerRepoService;

        public DelUserEH(ILogger<DelUserEvent> logger, IOfferRepoService userRepoService)
        {
            _logger = logger;
            _offerRepoService = userRepoService;
        }

        public Task Handle(DelUserEvent @event)
        {
            Console.WriteLine(@event.Name);
            var userGuid = new Guid(@event.Guid);
            //@event.ResponseReceivedEvent.Set();
            var offers = _offerRepoService.GetEntities().Where(x=>x.MasterGuid == userGuid);
            foreach (var offer in offers)
                _offerRepoService.DelEntity(offer.Guid);
            return Task.FromResult(true);
        }
    }
}
