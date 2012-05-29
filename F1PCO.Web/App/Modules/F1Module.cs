using Autofac;
using Autofac.Integration.Mvc;
using F1PCO.Integration.F1;

namespace F1PCO.Web.App.Modules
{
    public class F1Module : Module
    {
        public string ChurchCode { get; set; }
        public string ApiBaseUrlFormat { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<F1AuthorizationService>().As<IF1AuthorizationService>()
                .WithParameter("apiBaseUrl", GetApiBaseUrl())
                .WithParameter("consumerKey", ConsumerKey)
                .WithParameter("consumerSecret", ConsumerSecret)
                .InstancePerHttpRequest();

            builder.RegisterType<F1ClientProvider>().As<IF1ClientProvider>()
                .WithParameter("apiBaseUrl", GetApiBaseUrl())
                .WithParameter("consumerKey", ConsumerKey)
                .WithParameter("consumerSecret", ConsumerSecret)
                .InstancePerHttpRequest();

            builder.RegisterType<F1PersonRepository>().As<IF1PersonRepository>()
                .InstancePerHttpRequest();
        }

        private string GetApiBaseUrl()
        {
            return ApiBaseUrlFormat.Replace("{churchcode}", ChurchCode);
        }
    }
}