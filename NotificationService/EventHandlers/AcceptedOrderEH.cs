using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OrderEvents;
using Microsoft.Extensions.Logging;
using NotificationService.Abstractions;
using System.Threading.Tasks;

namespace NotificationService.EventHandlers
{
    public class AcceptedOrderEH : IIntegrationEventHandler<AcceptedOrderEvent>
    {
        private readonly ILogger<AcceptedOrderEH> _logger;
        private readonly INotificationRepoService _notificationRepoService;

        public AcceptedOrderEH(ILogger<AcceptedOrderEH> logger, INotificationRepoService notificationRepoService)
        {
            _logger = logger;
            _notificationRepoService = notificationRepoService;
        }

        public Task Handle(AcceptedOrderEvent @event)
        {
            _notificationRepoService.SendNotification(@event.MasterGuid, @event.ClientGuid, $"{@event.MasterName} accepted order!");
            return Task.CompletedTask;
        }
    }
}
