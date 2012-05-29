using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Module = Autofac.Module;

namespace Highway.Shared.Mvc
{
    public class WebApiModule : Module
    {
        private readonly Assembly[] _controllerAssembliesToScan;

        public WebApiModule(params Assembly[] controllerAssembliesToScan)
        {
            if (controllerAssembliesToScan == null || controllerAssembliesToScan.Any() == false)
            {
                _controllerAssembliesToScan = new[] { Assembly.GetCallingAssembly() };
            }
            else
            {
                _controllerAssembliesToScan = controllerAssembliesToScan;
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.ConfigureWebApi(GlobalConfiguration.Configuration);
            builder.RegisterApiControllers(_controllerAssembliesToScan);
        }
    }
}