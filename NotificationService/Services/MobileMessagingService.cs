using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using NotificationService.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class MobileMessagingService : IMobileMessagingService
    {
        private readonly FirebaseMessaging _messaging;

        public MobileMessagingService()
        {
            var app = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("beautylinkKey.json")
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
            });
            _messaging = FirebaseMessaging.GetMessaging(app);
        }

        private Message CreateNotification(string title, string notificationBody, string token) => new Message()
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification()
            {
                Body = notificationBody,
                Title = title
            }
        };

        public async Task SendNotification(string token, string title, string body)
        {
            try
            {
                var result = await _messaging.SendAsync(CreateNotification(title, body, token));
            }
            catch (Exception ex) { }
        }
    }
}
