using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.EventHandlers
{
    public class AddUserEH : IIntegrationEventHandler<AddUserEvent>
    {
        private readonly ILogger<AddUserEH> _logger;
        private readonly INotificationRepoService _notificationRepoService;

        public AddUserEH(ILogger<AddUserEH> logger, INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _notificationRepoService = notificationRepoService;
        }

        public Task Handle(AddUserEvent @event)
        {
            Console.WriteLine(@event.Name);
            //@event.ResponseReceivedEvent.Set();
            _notificationRepoService.AddEntity(new NotificationUser()
            {
                Name = @event.Name,
                UIDFB = new Guid(@event.Guid),
                Token = @event.Token
            });
            return Task.FromResult(0);
        }
    }
}
