using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace Highway.Shared.Mvc
{
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
                        var controller = (object)e.Instance as Controller;
                        if (controller == null)
                            return;
                        controller.TempDataProvider =
                            (ITempDataProvider)e.Context.ResolveService(tempDataProviderService);
                    });
        }
    }
}