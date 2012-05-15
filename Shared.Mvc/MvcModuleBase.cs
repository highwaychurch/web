using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.SessionState;
using Autofac;
using Highway.Shared.Mvc.TempData;

namespace Highway.Shared.Mvc
{
    public class MvcModuleBase : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var state = WebConfigurationManager.GetWebApplicationSection("system.web/sessionState") as SessionStateSection;
            
            if(state != null && state.Mode == SessionStateMode.Off)
                builder.RegisterType<CookieTempDataProvider>().As<ITempDataProvider>();
            else
                builder.RegisterType<SessionStateTempDataProvider>().As<ITempDataProvider>();
        }
    }
}