using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using System.Threading.Tasks;

namespace NotificationService.EventHandlers
{
    public class ExecutedOrderEH : IIntegrationEventHandler<ExecutedOrderEvent>
    {
        private readonly ILogger<ExecutedOrderEH> _logger;
        private readonly INotificationRepoService _notificationRepoService;

        public ExecutedOrderEH(ILogger<ExecutedOrderEH> logger, INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _notificationRepoService = notificationRepoService;
        }

        public Task Handle(ExecutedOrderEvent @event)
        {
            _notificationRepoService.SendNotification(@event.MasterGuid, @event.ClientGuid, $"{@event.MasterName} executed order!");
            return Task.CompletedTask;
        }
    }
}
