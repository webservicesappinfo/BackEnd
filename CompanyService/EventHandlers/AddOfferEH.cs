using CompanyService.Abstractions;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CompanyService.EventHandlers
{
    public class AddOfferEH : IIntegrationEventHandler<AddOfferEvent>
    {
        private readonly ILogger<AddOfferEH> _logger;
        private readonly ICompanyRepoService _companyRepoService;

        public AddOfferEH(ILogger<AddOfferEH> logger, ICompanyRepoService companyRepoService)
        {
            _logger = logger;
            _companyRepoService = companyRepoService;
        }

        public Task Handle(AddOfferEvent @event)
        {
            var result = _companyRepoService.OnAddOfferEvent(@event);
            return Task.FromResult(result);
        }
    }
}
