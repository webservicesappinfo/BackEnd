using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Abstractions
{
    public interface IMobileMessagingService
    {
        Task SendNotification(string token, string title, string body);
    }
}
