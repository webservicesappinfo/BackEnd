﻿using CompanyService.Abstractions;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyService.EventHandlers
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

            var user = new Guid(@event.Guid);

            var ownerCompanies = _companyRepoService.GetCompaniesByOwner(user);
            foreach (var c in ownerCompanies)
                _companyRepoService.DelEntity(c.Guid);

            var masterCompanies = _companyRepoService.GetCompaniesByWorker(user);
            foreach (var c in masterCompanies)
                _companyRepoService.DelWorker(c.Guid, user);

            return Task.FromResult(0);
        }
    }
}
