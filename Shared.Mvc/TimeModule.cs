using System.Web.Configuration;
using System.Web.SessionState;
using Autofac;
using Highway.Shared.Mvc.Time;
using Highway.Shared.Time;

namespace Highway.Shared.Mvc
{
    public class TimeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var state = WebConfigurationManager.GetWebApplicationSection("system.web/sessionState") as SessionStateSection;
            if (state != null && state.Mode == SessionStateMode.Off)
                builder.RegisterType<CookieClock>().As<IClock>();
            else
                builder.RegisterType<SessionClock>().AsSelf().As<IClock>();

        }
    }
}