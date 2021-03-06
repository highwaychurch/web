using Autofac;
using Autofac.Integration.Mvc;
using F1PCO.Integration.PCO;

namespace F1PCO.Web.App.Modules
{
    public class PCOModule : Module
    {
        public string ApiBaseUrl { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<PCOAuthorizationService>().As<IPCOAuthorizationService>()
                .WithParameter("apiBaseUrl", ApiBaseUrl)
                .WithParameter("consumerKey", ConsumerKey)
                .WithParameter("consumerSecret", ConsumerSecret)
                .InstancePerHttpRequest();

            builder.RegisterType<PCOClientProvider>().As<IPCOClientProvider>()
                .WithParameter("consumerKey", ConsumerKey)
                .WithParameter("consumerSecret", ConsumerSecret)
                .WithParameter("apiBaseUrl", ApiBaseUrl)
                .InstancePerHttpRequest();

            builder.RegisterType<PCOPersonRepository>().As<IPCOPersonRepository>()
                .InstancePerHttpRequest();
        }
    }
}