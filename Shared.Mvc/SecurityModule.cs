using Autofac;
using Highway.Shared.Mvc.Security;

namespace Highway.Shared.Mvc
{
    public class SecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<FormsAuthenticationService>().As<IFormsAuthenticationService>();
        }
    }
}
