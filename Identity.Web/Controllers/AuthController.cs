using System;
using System.Diagnostics;
using System.Web.Mvc;
using Highway.Identity.Web.App.Auth;
using Highway.Shared.Mvc.FlashMessages;

namespace Highway.Identity.Web.Controllers
{
    public class AuthController : Controller
    {
        [ValidateInput(false)]
        public ActionResult OpenId()
        {
            return HandleOpenId(Request.Form["openid_identifier"]);
        }

        public ActionResult Facebook()
        {
            var callback = new Uri(new Uri(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))), "/Auth/Callback?type=fb");

            new Facebook().Authenticate(callback);

            return new EmptyResult();
        }

        public ActionResult Google()
        {
            return HandleOpenId("https://www.google.com/accounts/o8/id");

        }

        public ActionResult Yahoo()
        {
            return HandleOpenId("http://yahoo.com/");


        }

        public ActionResult Twitter()
        {
            var callback = new Uri(new Uri(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"))), "/Auth/Callback?type=tw");

            new Twitter().Authenticate(callback);

            return new EmptyResult();
        }

        public ActionResult Callback(string type)
        {
            IOauthClient client;

            switch (type)
            {
                case "tw":
                    client = new Twitter();
                    break;
                case "fb":
                    client = new Facebook();
                    break;
                default:
                    return ErrorOut("No valid Oauth orchestration was found for current callback.");

            }

            var userInfo = client.Verify();

            return CompleteSignIn(userInfo);
        }

        private ActionResult ErrorOut(string message)
        {
            TempData.FlashError(message);
            return RedirectToAction("SignIn", "Authentication");
        }

        private ActionResult HandleOpenId(string oid)
        {
            try
            {
                var identity = new OpenId().ProcessOpenId(oid);
                if (identity == null) return new EmptyResult();

                return CompleteSignIn(identity);
            }
            catch (Exception ex)
            {
                return ErrorOut(ex.Message);
            }
        }

        private ActionResult CompleteSignIn(OpenIdentity identity)
        {
            if (identity == null || string.IsNullOrEmpty(identity.Username))
            {
                return ErrorOut("Invalid Identifier");
            }

            // Stick in next request or cookie!!!!!!!!!
            Debugger.Break();

            var cookie = Request.Cookies["returnUrl"];

            if (cookie != null)
            {
                return Redirect(cookie.Value);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
