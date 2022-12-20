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
    public class AddMasterEH : IIntegrationEventHandler<AddMasterEvent>
    {
        private readonly ILogger<AddMasterEH> _logger;
        private readonly IUserRepoService _userRepoService;

        public AddMasterEH(ILogger<AddMasterEH> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(AddMasterEvent @event)
        {
            Console.WriteLine(@event.CompanyGuid);
            //@event.ResponseReceivedEvent.Set();
            var user = _userRepoService.JoinToCompany(@event.MasterGuid, @event.CompanyGuid, @event.CompanyName);
            return Task.FromResult(0);
        }
    }
}
