using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using F1PCO.Integration.F1;
using F1PCO.OAuth;
using Highway.Shared.Mvc;
using Raven.Client;
using Raven.Client.Linq;

namespace F1PCO.Web.Controllers
{
    public class F1AuthController : AsyncController
    {
        private const string F1RequestTokenCookieKey = "F1RequestToken";
        private readonly IF1AuthorizationService _f1AuthorizationService;
        private readonly IAsyncDocumentSession _asyncSession;

        public F1AuthController(IF1AuthorizationService f1AuthorizationService, IAsyncDocumentSession asyncSession)
        {
            _f1AuthorizationService = f1AuthorizationService;
            _asyncSession = asyncSession;
        }

        public async Task<ActionResult> Authenticate()
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            var user = (await _asyncSession.Query<User>().Take(1).ToListAsync()).Single();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            if (user.F1AccessToken != null)
            {
                if (await _f1AuthorizationService.TryConnectWithPersistedAccessTokenAsync(user.F1AccessToken))
                {
                    // F1 is working with the persisted AccessToken so move on to PCO
                    return RedirectToAction("Authenticate", "PCOAuth");
                }
            }

            // Otherwise start the OAuth dance with F1
            var callbackUrl = Url.Action("CallBack", "F1Auth", null, Request.Url.Scheme);
            var requestToken = await _f1AuthorizationService.GetRequestTokenAsync(callbackUrl);
            Response.Cookies.SaveToCookie(F1RequestTokenCookieKey, requestToken);
            var oauthRedirect = _f1AuthorizationService.BuildPortalUserAuthorizationRequestUrl(requestToken, callbackUrl);
            return Redirect(oauthRedirect);
        }

        public async Task<ActionResult> CallBack()
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            var user = (await _asyncSession.Query<User>().Take(1).ToListAsync()).FirstOrDefault();
            if (user == null) throw new InvalidOperationException("There is no current user!");

            RequestToken requestToken;
            if (Request.Cookies.TryGetFromCookie(F1RequestTokenCookieKey, out requestToken) == false)
                throw new InvalidOperationException("The RequestToken could not be retrieved from the cookie.");

            var accessToken = await _f1AuthorizationService.GetAccessTokenAsync(requestToken);
            user.F1AccessToken = accessToken;
            Response.Cookies.Expire(F1RequestTokenCookieKey);

            return RedirectToAction("Authenticate", "PCOAuth");
        }
    }
}
