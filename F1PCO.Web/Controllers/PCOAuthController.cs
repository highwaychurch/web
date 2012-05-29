using System;
using System.Linq;
using System.Web.Mvc;
using F1PCO.Integration.PCO;
using F1PCO.OAuth;
using Highway.Shared.Mvc;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class PCOAuthController : Controller
    {
        private const string PCORequestTokenCookieKey = "PCORequestToken";
        private readonly IPCOAuthorizationService _pcoAuthorizationService;
        private readonly IDocumentSession _session;

        public PCOAuthController(IPCOAuthorizationService pcoAuthorizationService, IDocumentSession session)
        {
            _pcoAuthorizationService = pcoAuthorizationService;
            _session = session;
        }

        public ActionResult Authenticate()
        {
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            if (user.PCOAccessToken != null)
            {
                if (_pcoAuthorizationService.TryConnectWithPersistedAccessToken(user.PCOAccessToken))
                {
                    // PCO is working with the persisted AccessToken so move on to Ready
                    return RedirectToAction("Ready", "Home");
                }
            }

            // Otherwise start the OAuth dance with PCO
            var callbackUrl = Url.Action("CallBack", "PCOAuth", null, Request.Url.Scheme);
            var requestToken = _pcoAuthorizationService.GetRequestToken(callbackUrl);
            Response.Cookies.SaveToCookie(PCORequestTokenCookieKey, requestToken);
            var oauthRedirect = _pcoAuthorizationService.BuildAuthorizationRequestUrl(requestToken, callbackUrl);
            return Redirect(oauthRedirect);
        }

        public ActionResult CallBack(string oauth_verifier)
        {
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            RequestToken requestToken;
            if (Request.Cookies.TryGetFromCookie(PCORequestTokenCookieKey, out requestToken) == false)
                throw new InvalidOperationException("The RequestToken could not be retrieved from the cookie.");

            var accessToken = _pcoAuthorizationService.GetAccessToken(requestToken, oauth_verifier);

            user.PCOAccessToken = accessToken;

            return RedirectToAction("Ready", "Home");
        }
    }
}