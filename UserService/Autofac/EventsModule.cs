using Autofac;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UserService.EventHendlers;
using Module = Autofac.Module;

namespace UserService.Autofac
{
    public class EventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(AddCompanyEH).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
