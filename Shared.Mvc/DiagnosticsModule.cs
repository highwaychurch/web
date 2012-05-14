using System.Web.Mvc;
using Autofac;
using Highway.Shared.Diagnostics;
using Highway.Shared.Diagnostics.Log4Net;
using Highway.Shared.Mvc.Diagnostics;
using log4net.Config;

namespace Highway.Shared.Mvc
{
    public class DiagnosticsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            XmlConfigurator.Configure();

            builder.RegisterGeneric(typeof(Log4NetLog<>))
                .As(typeof(ILog<>))
                .SingleInstance();

            builder.RegisterType<ErrorHandlingFilter>().As<IExceptionFilter>();
            builder.RegisterType<RequestLoggingFilter>().As<IActionFilter>().SingleInstance();
        }
    }
}
