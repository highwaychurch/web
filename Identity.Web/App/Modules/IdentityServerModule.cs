using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Highway.Identity.Web.GlobalFilter;
using Highway.Identity.Web.Security;

namespace Highway.Identity.Web.App.Modules
{
    public class IdentityServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<GlobalViewModelFilter>().As<IActionFilter>().InstancePerHttpRequest();
            builder.RegisterType<SslRedirectFilter>().As<IActionFilter>().InstancePerHttpRequest();
            builder.RegisterType<InitialConfigurationFilter>().As<IActionFilter>().InstancePerHttpRequest();

            builder.RegisterType<AuthenticationHelper>().AsSelf().InstancePerHttpRequest();

        }
    }
}