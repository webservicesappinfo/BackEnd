using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using UserService.Abstractions;

namespace UserService.EventHandlers
{
    public class DelMasterEH : IIntegrationEventHandler<DelMasterEvent>
    {
        private readonly ILogger<DelMasterEH> _logger;
        private readonly IUserRepoService _userRepoService;

        public DelMasterEH(ILogger<DelMasterEH> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(DelMasterEvent @event)
        {
            Console.WriteLine(@event.CompanyGuid);
            //@event.ResponseReceivedEvent.Set();
            var user = _userRepoService.UnJoinCompany(@event.MasterUIDFB, @event.CompanyGuid);
            return Task.FromResult(0);
        }
    }
}
