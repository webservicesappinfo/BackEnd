using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserService.Abstractions;

namespace UserService.EventHandlers
{
    public class AddOfferEH : IIntegrationEventHandler<AddOfferEvent>
    {
        private readonly ILogger<AddOfferEvent> _logger;
        private readonly IUserRepoService _userRepoService;

        public AddOfferEH(ILogger<AddOfferEvent> logger, IUserRepoService userRepoService)
        {
            _logger = logger;
            _userRepoService = userRepoService;
        }

        public Task Handle(AddOfferEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            _userRepoService.AddOffer(@event.Guid, @event.Name, @event.MasterGuid);
            return Task.FromResult(0);
        }
    }
}
