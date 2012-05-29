using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.SessionState;
using Autofac;
using Autofac.Integration.Mvc;
using Highway.Shared.Mvc.TempData;
using Module = Autofac.Module;

namespace Highway.Shared.Mvc
{
    public class MvcModule : Module
    {
        private readonly Assembly[] _assembliesToScan;

        public MvcModule(params Assembly[] assembliesToScan)
        {
            if (assembliesToScan == null || assembliesToScan.Any() == false)
            {
                _assembliesToScan = new[] { Assembly.GetCallingAssembly() };
            }
            else
            {
                _assembliesToScan = assembliesToScan;
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var state = WebConfigurationManager.GetWebApplicationSection("system.web/sessionState") as SessionStateSection;

            if (state != null && state.Mode == SessionStateMode.Off)
                builder.RegisterType<CookieTempDataProvider>().As<ITempDataProvider>().InstancePerHttpRequest();
            else
                builder.RegisterType<SessionStateTempDataProvider>().As<ITempDataProvider>().InstancePerHttpRequest();

            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>().WithParameter("injectActionMethodParameters", true);
            builder.RegisterControllers(_assembliesToScan).InjectActionInvoker().InjectTempDataProvider();
            builder.RegisterModelBinders(_assembliesToScan);
            builder.RegisterModelBinderProvider();

            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterFilterProvider();
        }
    }
}