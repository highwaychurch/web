using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using F1PCO.Integration.PCO;
using F1PCO.OAuth;
using Highway.Shared.Mvc;
using Raven.Client;
using Raven.Client.Linq;

namespace F1PCO.Web.Controllers
{
    public class PCOAuthController : AsyncController
    {
        private const string PCORequestTokenCookieKey = "PCORequestToken";
        private readonly IPCOAuthorizationService _pcoAuthorizationService;
        private readonly IAsyncDocumentSession _asyncSession;

        public PCOAuthController(IPCOAuthorizationService pcoAuthorizationService, IAsyncDocumentSession asyncSession)
        {
            _pcoAuthorizationService = pcoAuthorizationService;
            _asyncSession = asyncSession;
        }

        public async Task<ActionResult> Authenticate()
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            var user = (await _asyncSession.Query<User>().Take(1).ToListAsync()).Single();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            if (user.PCOAccessToken != null)
            {
                if (await _pcoAuthorizationService.TryConnectWithPersistedAccessTokenAsync(user.PCOAccessToken))
                {
                    // PCO is working with the persisted AccessToken so move on to Ready
                    return RedirectToAction("Ready", "Home");
                }
            }

            // Otherwise start the OAuth dance with PCO
            var callbackUrl = Url.Action("CallBack", "PCOAuth", null, Request.Url.Scheme);
            var requestToken = await _pcoAuthorizationService.GetRequestTokenAsync(callbackUrl);
            Response.Cookies.SaveToCookie(PCORequestTokenCookieKey, requestToken);
            var oauthRedirect = _pcoAuthorizationService.BuildAuthorizationRequestUrl(requestToken, callbackUrl);
            return Redirect(oauthRedirect);
        }

        public async Task<ActionResult> CallBack(string oauth_verifier)
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            var user = (await _asyncSession.Query<User>().Take(1).ToListAsync()).Single();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            RequestToken requestToken;
            if (Request.Cookies.TryGetFromCookie(PCORequestTokenCookieKey, out requestToken) == false)
                throw new InvalidOperationException("The RequestToken could not be retrieved from the cookie.");

            var accessToken = await _pcoAuthorizationService.GetAccessTokenAsync(requestToken, oauth_verifier);
            user.PCOAccessToken = accessToken;
            Response.Cookies.Expire(PCORequestTokenCookieKey);

            return RedirectToAction("Ready", "Home");
        }
    }
}