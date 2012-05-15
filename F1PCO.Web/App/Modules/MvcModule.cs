using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Highway.Shared.Mvc;

namespace F1PCO.Web.App.Modules
{
    public class MvcModule : MvcModuleBase
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
}
