using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBus.RabbitMQ;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using EventBus.ServicesEvents.MobileClientEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Autofac;
using NotificationService.EventHandlers;
using NotificationService.Abstractions;
using NotificationService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            EventBusService.AddEventBus(services, "NotificationService");

            services.AddGrpc();
            services.AddSingleton<IMobileMessagingService, MobileMessagingService>();
            services.AddScoped<INotificationRepoService, NotificationRepoService>();

            //EventBusService.AddEventBus(services);
        }

        //Autofac registry types
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new EventsModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<NotificationServiceImp>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });


            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<AddUserEvent, AddUserEH>();
            eventBus.Subscribe<DelUserEvent, DelUserEH>();
        }
    }
}
