using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.OrderEvents;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using System.Threading.Tasks;

namespace OfferService.EventHandlers
{
    public class GetInfoFromCompany : IIntegrationEventHandler<SendInfoForOffer>
    {
        private readonly ILogger<SendInfoForOffer> _logger;
        private readonly IOfferRepoService _offerRepoService;
        public GetInfoFromCompany(ILogger<SendInfoForOffer> logger, IOfferRepoService offerRepoService)
        {
            _logger = logger;
            _offerRepoService = offerRepoService;
        }

        public Task Handle(SendInfoForOffer @event)
        {
            var result = _offerRepoService.SetInfoFromCompany(@event);
            return Task.FromResult(result);
        } 
    }
}
