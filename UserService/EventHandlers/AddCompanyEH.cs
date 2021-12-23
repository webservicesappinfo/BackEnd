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
    public class AddCompanyEH : IIntegrationEventHandler<AddCompanyEvent>
    {
        private readonly ILogger<AddCompanyEvent> _logger;
        private readonly IUserRepoService _userRepoService;

        public AddCompanyEH(ILogger<AddCompanyEvent> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(AddCompanyEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            _userRepoService.AddCompany(@event.Owner, @event.Guid, @event.Name);
            return Task.FromResult(0);
        }
    }
}
