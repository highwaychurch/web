using Autofac;
using F1PCO.Web.Integration.F1;

namespace F1PCO.Web.App.Modules
{
    public class F1Module : Module
    {
        public string ChurchCode { get; set; }
        public string ApiBaseUrlFormat { get; set; }
        public string PortalUserAuthorizeUrlFormat { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<F1AuthorizationService>().As<IF1AuthorizationService>()
                .WithParameter("apiBaseUrl", ApiBaseUrlFormat.Replace("{churchcode}", ChurchCode))
                .WithParameter("portalUserAuthorizeUrlFormat", PortalUserAuthorizeUrlFormat.Replace("{churchcode}", ChurchCode))
                .WithParameter("consumerKey", ConsumerKey)
                .WithParameter("consumerSecret", ConsumerSecret);

            builder.RegisterType<F1ClientProvider>().As<IF1ClientProvider>()
                .WithParameter("apiBaseUrl", ApiBaseUrlFormat.Replace("{churchcode}", ChurchCode));

            builder.RegisterType<F1PersonRepository>().As<IF1PersonRepository>();
        }
    }
}