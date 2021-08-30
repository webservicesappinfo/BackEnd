using Autofac;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using LocationService.Abstractions;
using LocationService.Autofac;
using LocationService.EventHandlers;
using LocationService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using EventBus.ServicesEvents.MobileClientEvents;

namespace LocationService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            EventBusService.AddEventBus(services, "LocalService");

            services.AddGrpc();
            services.AddScoped<ILocationRepoService, LocationRepoService>();

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
                endpoints.MapGrpcService<LocationRepoImp>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<AddUserEvent, AddUserEH>();
            eventBus.Subscribe<TestBusEvent, TestBusEventHandler>();
        }
    }
}
