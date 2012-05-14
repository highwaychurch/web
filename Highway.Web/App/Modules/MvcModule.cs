using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;

namespace Highway.Web.App.Modules
{
    public class MvcModule : Shared.Mvc.MvcModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var thisAssembly = typeof (MvcModule).Assembly;

            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>().WithParameter("injectActionMethodParameters", true);
            builder.RegisterControllers(thisAssembly).InjectActionInvoker().InjectTempDataProvider();
            builder.RegisterModelBinders(thisAssembly);
            builder.RegisterModelBinderProvider();

            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterFilterProvider();
        }
    }

    public static class AutofacModuleExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InjectTempDataProvider<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder)
        {
            return registrationBuilder.InjectTempDataProvider(new TypedService(typeof(ITempDataProvider)));
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InjectTempDataProvider<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder, Service tempDataProviderService)
        {
            if (registrationBuilder == null)
                throw new ArgumentNullException("registrationBuilder");
            if (tempDataProviderService == null)
                throw new ArgumentNullException("tempDataProviderService");

            return registrationBuilder.OnActivating(
                e =>
                    {
                        var controller = (object) e.Instance as Controller;
                        if (controller == null)
                            return;
                        controller.TempDataProvider =
                            (ITempDataProvider) e.Context.ResolveService(tempDataProviderService);
                    });
        }
    }
}
