using System;
using System.Linq;
using System.Web.Mvc;
using F1PCO.OAuth;
using F1PCO.Web.Integration.F1;
using Highway.Shared.Mvc;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class F1AuthController : Controller
    {
        private const string F1RequestTokenCookieKey = "F1RequestToken";
        private readonly IF1AuthorizationService _f1AuthorizationService;
        private readonly IDocumentSession _session;

        public F1AuthController(IF1AuthorizationService f1AuthorizationService, IDocumentSession session)
        {
            _f1AuthorizationService = f1AuthorizationService;
            _session = session;
        }

        public ActionResult Authenticate()
        {
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            if (user.F1AccessToken != null)
            {
                if (_f1AuthorizationService.TryConnectWithPersistedAccessToken(user.F1AccessToken))
                {
                    // F1 is working with the persisted AccessToken so move on to PCO
                    return RedirectToAction("Authenticate", "PCOAuth");
                }
            }
           
            // Otherwise start the OAuth dance with F1
            var callbackUrl = Url.Action("CallBack", "F1Auth", null, Request.Url.Scheme);
            var requestToken = _f1AuthorizationService.GetRequestToken(callbackUrl);
            Response.Cookies.SaveToCookie(F1RequestTokenCookieKey, requestToken);
            var oauthRedirect = _f1AuthorizationService.BuildPortalUserAuthorizationRequestUrl(requestToken, callbackUrl);
            return Redirect(oauthRedirect);
        }

        public ActionResult CallBack()
        {
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            RequestToken requestToken;
            if (Request.Cookies.TryGetFromCookie(F1RequestTokenCookieKey, out requestToken) == false)
                throw new InvalidOperationException("The RequestToken could not be retrieved from the cookie.");

            var accessToken = _f1AuthorizationService.GetAccessToken(requestToken);

            user.F1AccessToken = accessToken;

            return RedirectToAction("Authenticate", "PCOAuth");
        }
    }
}
