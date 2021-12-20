using Autofac;
using EventBus.Abstractions;
using OfferService.EventHendlers;
using System.Reflection;
using Module = Autofac.Module;

namespace OfferService.Autofac
{
    public class OfferEventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(AddOrderEH).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
