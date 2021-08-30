using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using LocationService.Abstractions;
using LocationService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService.EventHandlers
{
    public class AddUserEH : IIntegrationEventHandler<AddUserEvent>
    {
        private readonly ILogger<AddUserEH> _logger;
        private readonly ILocationRepoService _locationRepoService;

        public AddUserEH(ILogger<AddUserEH> logger, ILocationRepoService locationRepoService)
        {
            _logger = logger;
            _locationRepoService = locationRepoService;
        }

        public Task Handle(AddUserEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            _locationRepoService.AddUser(new LocationUser()
            {
                Name = @event.Name,
                Guid = @event.Guid,
            });
            return Task.FromResult(0);
        }
    }
}
