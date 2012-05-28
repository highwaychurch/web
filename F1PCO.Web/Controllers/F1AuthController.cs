using System.Linq;
using System.Web.Mvc;
using F1PCO.Web.Integration.F1;
using F1PCO.Web.Models;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class F1AuthController : Controller
    {
        private readonly IF1AuthorizationService _f1AuthorizationService;

        public F1AuthController(IF1AuthorizationService f1AuthorizationService)
        {
            _f1AuthorizationService = f1AuthorizationService;
        }

        public ActionResult Authenticate(IDocumentSession session)
        {
            if (_f1AuthorizationService.IsAuthorized)
            {
                return RedirectToAction("Authenticate", "PCOAuth");
            }

            var persistedF1AccessToken = session.Query<PersistedF1Token>().FirstOrDefault();
            if (persistedF1AccessToken != null)
            {
                if (_f1AuthorizationService.TryAuthorizeWithPersistedAccessToken(persistedF1AccessToken.AccessToken))
                {
                    return RedirectToAction("Authenticate", "PCOAuth");
                }
            }
           
            var callbackUrl = Url.Action("CallBack", "F1Auth", null, Request.Url.Scheme);
            return Redirect(_f1AuthorizationService.BuildPortalUserAuthorizationRequestUrl(callbackUrl));
        }

        public ActionResult CallBack(IDocumentSession session)
        {
            var accessToken = _f1AuthorizationService.RequestAndPersistAccessToken();
            var persistedAccessToken = session.Query<PersistedF1Token>().FirstOrDefault();
            if (persistedAccessToken != null)
            {
                persistedAccessToken.AccessToken = accessToken;
            }
            else
            {
                persistedAccessToken = new PersistedF1Token {AccessToken = accessToken};
                session.Store(persistedAccessToken);
            }

            return RedirectToAction("Authenticate", "PCOAuth");
        }
    }
}
