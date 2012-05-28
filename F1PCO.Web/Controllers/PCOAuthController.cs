using System.Linq;
using System.Web.Mvc;
using F1PCO.Web.Integration.PCO;
using F1PCO.Web.Models;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class PCOAuthController : Controller
    {
        private readonly IPCOAuthorizationService _pcoAuthorizationService;

        public PCOAuthController(IPCOAuthorizationService pcoAuthorizationService)
        {
            _pcoAuthorizationService = pcoAuthorizationService;
        }

        public ActionResult Authenticate(IDocumentSession session)
        {
            if (_pcoAuthorizationService.IsAuthorized)
            {
                return RedirectToAction("Ready", "Home");
            }

            var persistedpcoAccessToken = session.Query<PersistedPCOToken>().FirstOrDefault();
            if (persistedpcoAccessToken != null)
            {
                if (_pcoAuthorizationService.TryAuthorizeWithPersistedAccessToken(persistedpcoAccessToken.AccessToken))
                {
                    return RedirectToAction("Ready", "Home");
                }
            }

            var callbackUrl = Url.Action("CallBack", "PCOAuth", null, Request.Url.Scheme);
            return Redirect(_pcoAuthorizationService.BuildPortalUserAuthorizationRequestUrl(callbackUrl));
        }

        public ActionResult CallBack(IDocumentSession session)
        {
            var accessToken = _pcoAuthorizationService.RequestAndPersistAccessToken();
            var persistedAccessToken = session.Query<PersistedPCOToken>().FirstOrDefault();
            if (persistedAccessToken != null)
            {
                persistedAccessToken.AccessToken = accessToken;
            }
            else
            {
                persistedAccessToken = new PersistedPCOToken {AccessToken = accessToken};
                session.Store(persistedAccessToken);
            }

            return RedirectToAction("Ready", "Home");
        }
    }
}