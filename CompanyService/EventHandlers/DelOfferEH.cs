using CompanyService.Abstractions;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyService.EventHandlers
{
    public class DelOfferEH : IIntegrationEventHandler<DelOfferEvent>
    {
        private readonly ILogger<DelOfferEvent> _logger;
        private readonly ICompanyRepoService _companyRepoService;

        public DelOfferEH(ILogger<DelOfferEvent> logger, ICompanyRepoService companyRepoService)
        {
            _logger = logger;
            _companyRepoService = companyRepoService;
        }

        public Task Handle(DelOfferEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            var result = _companyRepoService.OnDelOfferEvent(@event);
            return Task.FromResult(result);
        }
    }
}
