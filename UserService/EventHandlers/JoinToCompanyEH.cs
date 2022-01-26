using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Abstractions;

namespace UserService.EventHandlers
{
    public class JoinToCompanyEH : IIntegrationEventHandler<JoinToCompanyEvent>
    {
        private readonly ILogger<JoinToCompanyEvent> _logger;
        private readonly IUserRepoService _userRepoService;

        public JoinToCompanyEH(ILogger<JoinToCompanyEvent> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(JoinToCompanyEvent @event)
        {
            Console.WriteLine(@event.CompanyGuid);
            //@event.ResponseReceivedEvent.Set();
            //var user = _userRepoService.AddCompany(@event.MasterGuid, @event.CompanyGuid, @event.CompanyName);
            return Task.FromResult(0);
        }
    }
}
