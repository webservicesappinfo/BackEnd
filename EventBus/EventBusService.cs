using Autofac;
using EventBus.Abstractions;
using EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class EventBusService
    {
        public static void AddEventBus(IServiceCollection services, String subscriptionClientName)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
                        ?? Environment.GetEnvironmentVariable("EVENTBUS_HOST");

                if (string.IsNullOrWhiteSpace(host))
                {
                    bool isRunningInDocker = string.Equals(
                        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                        "true",
                        StringComparison.OrdinalIgnoreCase);

                    host = isRunningInDocker ? "rabbitmq" : "localhost";
                }

                var portEnv = Environment.GetEnvironmentVariable("RABBITMQ_PORT")
                           ?? Environment.GetEnvironmentVariable("EVENTBUS_PORT");
                int port = int.TryParse(portEnv, out var p) ? p : 5672;

                var user = Environment.GetEnvironmentVariable("RABBITMQ_USER")
                        ?? Environment.GetEnvironmentVariable("EVENTBUS_USER")
                        ?? "guest";

                var password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
                            ?? Environment.GetEnvironmentVariable("EVENTBUS_PASSWORD")
                            ?? "guest";

                var factory = new ConnectionFactory()
                {
                    HostName = host,
                    Port = port,
                    UserName = user,
                    Password = password,
                    DispatchConsumersAsync = true
                };
                var retryCount = 5;
                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, DefaultSubscriptionsManager>();

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
        }
    }
}
