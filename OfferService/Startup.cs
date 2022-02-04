using Autofac;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.OrderEvents;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfferService.Abstractions;
using OfferService.Autofac;
using OfferService.EventHandlers;
using OfferService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfferService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddScoped<IOfferRepoService, OfferRepoService>();

            EventBusService.AddEventBus(services, "OfferService");
        }

        //Autofac registry types
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new OfferEventsModule());
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
                endpoints.MapGrpcService<OfferServiceImp>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<AddOrderEvent, AddOrderEH>();
            eventBus.Subscribe<DelOrderEvent, DelOrderEH>();
            eventBus.Subscribe<DelUserEvent, DelUserEH>();
            eventBus.Subscribe<SendInfoForOffer, GetInfoFromCompany>();

        }
    }
}
