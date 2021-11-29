using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Abstractions;

namespace UserService.EventHendlers
{
    public class DelCompanyEH : IIntegrationEventHandler<DelCompanyEvent>
    {
        private readonly ILogger<DelCompanyEvent> _logger;
        private readonly IUserRepoService _userRepoService;

        public DelCompanyEH(ILogger<DelCompanyEvent> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(DelCompanyEvent @event)
        {
            Console.WriteLine(@event.Guid);
            //@event.ResponseReceivedEvent.Set();
            var usersWithCompany = _userRepoService.GetEntities().Where(x => x.Companies.Any(x => x.RefGuid == @event.Guid)).ToList();
            foreach (var user in usersWithCompany)
                _userRepoService.DelCompany(user.UIDFB, @event.Guid);
            return Task.FromResult(0);
        }
    }
}
