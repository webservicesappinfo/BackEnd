using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using System.Threading.Tasks;

namespace NotificationService.EventHandlers
{
    public class AddOrderEH : IIntegrationEventHandler<AddOrderEvent>
    {
        private readonly ILogger<AddOrderEH> _logger;
        private readonly INotificationRepoService _notificationRepoService;

        public AddOrderEH(ILogger<AddOrderEH> logger, INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _notificationRepoService = notificationRepoService;
        }

        public Task Handle(AddOrderEvent @event)
        {
            _notificationRepoService.SendNotification(@event.ClientGuid, @event.MasterGuid, $"{@event.ClientName} created order!");
            return Task.CompletedTask;
        }
    }
}
