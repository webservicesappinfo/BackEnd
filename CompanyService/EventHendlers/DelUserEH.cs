using CompanyService.Abstractions;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyService.EventHendlers
{
    public class DelUserEH : IIntegrationEventHandler<DelUserEvent>
    {
        private readonly ILogger<DelUserEvent> _logger;
        private readonly ICompanyRepoService _companyRepoService;

        public DelUserEH(ILogger<DelUserEvent> logger, ICompanyRepoService companyRepoService)
        {
            _logger = logger;
            _companyRepoService = companyRepoService;
        }

        public Task Handle(DelUserEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();

            var companies = _companyRepoService.GetCompaniesByOwner(new Guid(@event.Guid));
            foreach (var c in companies)
                _companyRepoService.DelCompany(c);
            return Task.FromResult(0);
        }
    }
}
