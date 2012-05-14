using Autofac;
using Highway.Identity.Core.TokenService;

namespace Highway.Identity.Web.App.Modules
{
    public class WindowsIdentityFoundationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<TokenServiceHostFactory>().AsSelf();
            builder.RegisterType<TokenServiceConfiguration>().AsSelf();
        }
    }
}
