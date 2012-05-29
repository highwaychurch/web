using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activation;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Configuration;
using Autofac.Integration.Mvc;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.TokenService;
using Highway.Identity.Web.App.Modules;
using Highway.Identity.Web.Security;
using Highway.Shared.Diagnostics;
using Highway.Shared.Mvc;
using Highway.Shared.Mvc.Validation;
using Microsoft.IdentityModel.Web;

namespace Highway.Identity.Web
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

        protected void Application_Start()
        {
            if (IsDebug) Thread.Sleep(2000);  // Wait for RavenDB to start in debug

            // Build the container
            var builder = new ContainerBuilder();
            builder.RegisterModule<WindowsIdentityFoundationModule>();
            builder.RegisterModule<IdentityServerModule>();
            builder.RegisterModule(new MvcModule());
            builder.RegisterModule<PersistenceModule>();
            builder.RegisterModule<SecurityModule>();
            builder.RegisterModule<DiagnosticsModule>();
            builder.RegisterModule<TimeModule>();
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            _log = _container.Resolve<ILog<MvcApplication>>();
            _log.Information("Application_Start");

            // Hook up all of the WIF bits using IoC/DI where possible
            InitializeWindowsIdentityFoundationBits();
    
            // And do all of the MVC stuff
            AreaRegistration.RegisterAllAreas();
            // We use JSON.NET instead
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().First());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterValidators();
        }

        static void InitializeWindowsIdentityFoundationBits()
        {
            // Yucky WIF stuff
            RepositoryX509SecurityTokenHandler.UserRepositoryFactoryMethod =
                () => DependencyResolver.Current.GetService<IUserRepository>();
            RepositoryUserNameSecurityTokenHandler.UserRepositoryFactoryMethod =
                () => DependencyResolver.Current.GetService<IUserRepository>();
            AuthorizationManager.ConfigurationRepositoryFactoryMethod =
                () => DependencyResolver.Current.GetService<IConfigurationRepository>();
            ClaimsTransformer.UserRepositoryFactoryMethod =
                () => DependencyResolver.Current.GetService<IUserRepository>();
            FederatedAuthentication.ServiceConfigurationCreated +=
                (s, e) =>
                    {
                        var configurationRepository = DependencyResolver.Current.GetService<IConfigurationRepository>();
                        if (
                            !string.IsNullOrWhiteSpace(
                                configurationRepository.SigningCertificate.SubjectDistinguishedName))
                        {
                            e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(
                                new X509CertificateSessionSecurityTokenHandler(
                                    configurationRepository.SigningCertificate.Certificate));
                        }
                    };
        }

        static void RegisterValidators()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RequiredIfAttribute),
                typeof(RequiredIfValidator));
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // All other filters are added as part of other Autofac modules
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("css/{*path}");
            routes.IgnoreRoute("js/{*path}");
            routes.IgnoreRoute("img/{*path}");
            routes.IgnoreRoute("bootstrap/{*path}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });

            routes.MapRoute(
                "FederationMetadata",
                "FederationMetadata/2007-06/FederationMetadata.xml",
                new { controller = "FederationMetadata", action = "Generate" }
            );

            routes.MapRoute(
                "RelyingPartiesAdmin",
                "admin/relyingparties/{action}/{id}",
                new { controller = "RelyingPartiesAdmin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ClientCertificatesAdmin",
                "admin/clientcertificates/{action}/{userName}",
                new { controller = "ClientCertificatesAdmin", action = "Index", userName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "DelegationAdmin",
                "admin/delegation/{action}/{userName}",
                new { controller = "DelegationAdmin", action = "Index", userName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { controller = "^(?!issue).*" }
            );

            // ws-federation (mvc)
            routes.MapRoute(
                "wsfederation",
                "issue/wsfed",
                new { controller = "WSFederation", action = "issue" }
            );

            // jsnotify (mvc)
            routes.MapRoute(
                "jsnotify",
                "issue/jsnotify",
                new { controller = "JSNotify", action = "issue" }
            );

            // simple http (mvc)
            routes.MapRoute(
                "simplehttp",
                "issue/simple",
                new { controller = "SimpleHttp", action = "issue" }
            );

            // oauth wrap (mvc)
            routes.MapRoute(
                "wrap",
                "issue/wrap",
                new { controller = "Wrap", action = "issue" }
            );

            // oauth2 (mvc)
            routes.MapRoute(
                "oauth2",
                "issue/oauth2/{action}",
                new { controller = "OAuth2", action = "token" }
            );

            // ws-trust (wcf)
            routes.Add(new ServiceRoute(
                "issue/wstrust",
                DependencyResolver.Current.GetService<TokenServiceHostFactory>(),
                typeof(TokenServiceConfiguration)));
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var context = (sender as HttpApplication).Context;

            if (context.Response.StatusCode == 401)
            {
                var noRedirect = context.Items["NoRedirect"];

                if (noRedirect == null)
                {
                    var route = new RouteValueDictionary(new Dictionary<string, object>
                        {
                            { "Controller", "Authentication" },
                            { "Action", "SignIn" },
                            { "ReturnUrl", HttpUtility.UrlEncode(context.Request.RawUrl, context.Request.ContentEncoding) }
                        });

                    Response.RedirectToRoute(route);
                }
            }
        }

        protected void Application_End()
        {
            if (_log != null) _log.Information("Application_End");
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