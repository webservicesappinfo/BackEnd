using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.EventHandlers
{
    public class DelUserEH : IIntegrationEventHandler<DelUserEvent>
    {
        private readonly ILogger<DelUserEvent> _logger;
        private readonly INotificationRepoService _notificationRepoService;

        public DelUserEH(ILogger<DelUserEvent> logger, INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _notificationRepoService = notificationRepoService;
        }

        public Task Handle(DelUserEvent @event)
        {
            Console.WriteLine(@event.Name);
            var userGuid = new Guid(@event.Guid);
            //@event.ResponseReceivedEvent.Set();
            var user = _notificationRepoService.GetEntities().FirstOrDefault(x => x.UIDFB == userGuid);
            if (user != null)
            {
                _notificationRepoService.DelEntity(user.Guid);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
