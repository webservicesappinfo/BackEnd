using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using EventBus.Abstractions;
using NotificationService.Handlers;
using Module = Autofac.Module;

namespace NotificationService.Autofac
{
    public class EventcModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(TestBusEventHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
