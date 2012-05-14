using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Configuration;
using Autofac.Integration.Mvc;
using Highway.Shared.Diagnostics;
using Highway.Shared.Mvc;
using Highway.Shared.Mvc.Validation;

namespace Creative.Web
{
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
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("css/{*path}");
            routes.IgnoreRoute("js/{*path}");
            routes.IgnoreRoute("img/{*path}");
            routes.IgnoreRoute("bootstrap/{*path}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            // Build the container
            var builder = new ContainerBuilder();
            builder.RegisterModule<ConfigurationSettingsReader>();
            builder.RegisterModule<App.Modules.MvcModule>();
            builder.RegisterModule<App.Modules.PersistenceModule>();
            builder.RegisterModule<SecurityModule>();
            builder.RegisterModule<DiagnosticsModule>();
            builder.RegisterModule<TimeModule>();
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            _log = _container.Resolve<ILog<MvcApplication>>();
            _log.Information("Application_Start");

            // And do all of the MVC stuff
            AreaRegistration.RegisterAllAreas();
            // We use JSON.NET instead
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().First());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof (RequiredIfAttribute),
                typeof (RequiredIfValidator));
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