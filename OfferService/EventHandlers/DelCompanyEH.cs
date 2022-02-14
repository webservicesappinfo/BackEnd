using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService.EventHandlers
{
    public class DelCompanyEH : IIntegrationEventHandler<DelCompanyEvent>
    {
        private readonly ILogger<DelCompanyEvent> _logger;
        private readonly IOfferRepoService _offerRepoService;

        public DelCompanyEH(ILogger<DelCompanyEvent> logger, IOfferRepoService userRepoService)
        {
            _logger = logger;
            _offerRepoService = userRepoService;
        }

        public Task Handle(DelCompanyEvent @event)
        {
            Console.WriteLine($"DelCompanyEvent guid: {@event.Guid}");
            //@event.ResponseReceivedEvent.Set();
            var result = _offerRepoService.OnDelCompanyEH(@event.Guid);
            return Task.FromResult(result);
        }
    }

}
