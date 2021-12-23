using Autofac;
using EventBus.Abstractions;
using OrderService.EventHandlers;
using System.Reflection;
using Module = Autofac.Module;

namespace OrderService.Autofac
{
    public class OrderEventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(DelOfferEH).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            builder.RegisterAssemblyTypes(typeof(DelUserEH).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>)); 
        }
    }
}
