using Autofac;
using F1PCO.Web.Integration.PCO;

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
                .WithParameter("consumerSecret", ConsumerSecret);

            builder.RegisterType<PCOClientProvider>().As<IPCOClientProvider>()
                .WithParameter("apiBaseUrl", ApiBaseUrl);

            builder.RegisterType<PCOPersonRepository>().As<IPCOPersonRepository>();
        }
    }
}