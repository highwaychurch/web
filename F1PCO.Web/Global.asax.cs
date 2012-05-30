using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using F1PCO.Web.App.Modules;
using Highway.Shared.Autofac;
using Highway.Shared.Diagnostics;
using Highway.Shared.Mvc;
using Highway.Shared.Mvc.Persistence;
using Highway.Shared.Mvc.Validation;
using Module = Autofac.Module;

namespace F1PCO.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
#if DEBUG
        public const bool IsDebug = true;
#else
        public const bool IsDebug = false;
#endif

        static Version _fileVersion;
        ILog<MvcApplication> _log;
        IContainer _container;

        /// <summary>
        /// Gets FileVersion (not .NET assembly version, i.e. not webAssembly.GetName().Version)
        /// </summary>
        public static Version Version
        {
            get
            {
                if (_fileVersion == null)
                {
                    var webAssembly = Assembly.GetExecutingAssembly();
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(webAssembly.Location);
                    _fileVersion = new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
                }
                return _fileVersion;
            }
        }

        public static string ServerHost
        {
            get { return HttpContext.Current.Server.MachineName; }
        }

        public static bool ShowDebugComponents
        {
            get
            {
#pragma warning disable 162
                return IsDebug;

                //var context = System.Web.HttpContext.Current;
                //if (context == null || context.Request == null)
                //    return false;
                //var cookie = context.Request.Cookies[DebugController.DebugCookieName];
                //return cookie != null && cookie.Value == DebugController.DebugCookieValue;
#pragma warning restore 162
            }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("cachedassets/{*path}");
            routes.IgnoreRoute("Content/{*path}");
            routes.IgnoreRoute("Scripts/{*path}");
            routes.IgnoreRoute("bootstrap/{*path}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });

            // Ensure the WebApi route is first so the MVC route doesn't 404 us on a missing "api" controller
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            if (IsDebug) Thread.Sleep(2000);  // Wait for RavenDB to start in debug

            // Build the container
            var modules = new List<Module>
                              {
                                  new MvcModule(),
                                  new WebApiModule(),
                                  new RavenPersistenceModule(),
                                  new SecurityModule(),
                                  new DiagnosticsModule(),
                                  new TimeModule(),
                                  new F1Module(),
                                  new PCOModule()
                              };

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfiguredModules(modules));
            _container = builder.Build();

            _log = _container.Resolve<ILog<MvcApplication>>();
            _log.Information("Application_Start");

            ConfigureMvc();
            ConfigureWebApi();
        }

        private void ConfigureMvc()
        {
            // Register the container as the MVC DependencyResolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));

            // Force JSON.NET instead
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().Single());
            ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RequiredIfAttribute),
                typeof(RequiredIfValidator));
        }

        private void ConfigureWebApi()
        {
            // Register the container as the WebApi ServiceResolver
            GlobalConfiguration.Configuration.ServiceResolver.SetResolver(new AutofacWebApiDependencyResolver(_container));

            // Force JSON.NET instead
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.OfType<JsonMediaTypeFormatter>().Single());
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonNetFormatter());
        }

        protected void Application_End()
        {
            _log.Information("Application_End");
        }

        public override void Init()
        {
            base.Init();

            Error += OnError;
        }

        private void OnError(object sender, EventArgs e)
        {
            try
            {
                var exception = Server.GetLastError();

                // Don't log client http errors.
                var httpException = exception as HttpException;
                if (httpException != null && httpException.GetHttpCode() >= 400 && httpException.GetHttpCode() <= 499)
                    return;

                if (_log != null)
                    _log.Fatal(Server.GetLastError(), "Exception caught in global exception handler");
            }
            catch
            {
                // Intentionally left blank
            }
        }
    }
}