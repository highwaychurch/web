using System.Collections.Generic;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;

namespace Highway.Identity.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;

        public HomeController(IConfigurationRepository configuration)
        {
            _configurationRepository = configuration;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AppIntegration()
        {
            var endpoints = Endpoints.Create(
                HttpContext.Request.Headers["Host"],
                HttpContext.Request.ApplicationPath,
                _configurationRepository.Endpoints.HttpPort,
                _configurationRepository.Endpoints.HttpsPort);

            var list = new Dictionary<string, string>
                           {
                               {"WS-Federation", endpoints.WSFederation.AbsoluteUri},
                               {"WS-Federation metadata", endpoints.WSFederationMetadata.AbsoluteUri},

                               {"WS-Trust mixed (UserName)", endpoints.WSTrustMixedUserName.AbsoluteUri},
                               {"WS-Trust mixed (Certificate)", endpoints.WSTrustMixedCertificate.AbsoluteUri},
                               {"WS-Trust message (UserName)", endpoints.WSTrustMessageUserName.AbsoluteUri},
                               {"WS-Trust message (Certificate)", endpoints.WSTrustMessageCertificate.AbsoluteUri},
                               {"WS-Trust metadata", endpoints.WSTrustMex.AbsoluteUri},

                               {"WRAP", endpoints.Wrap.AbsoluteUri},
                               {"OAuth2", endpoints.OAuth2.AbsoluteUri},
                               {"JSNotify", endpoints.JSNotify.AbsoluteUri},
                               {"Simple HTTP", endpoints.SimpleHttp.AbsoluteUri},
                           };

            return View(list);
        }
    }
}