using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1OAuthTest.Integration.FellowshipOne;

namespace F1OAuthTest.Controllers
{
    public class F1AuthController : Controller
    {
        private F1AuthorizationHelper _f1AuthHelper;

        private void Load()
        {
            _f1AuthHelper = new F1AuthorizationHelper(Request, Response, churchCode: "hwychrau", consumerKey: "429",
                                                      consumerSecret: "cc0f53b6-8057-4800-94e1-5fafe00b5509");
        }

        public ActionResult Authenticate()
        {
            Load();
            if (_f1AuthHelper.ExistingAccessToken != null)
            {
                return RedirectToAction("Ready");
            }
            
            var requestToken = _f1AuthHelper.RequestRequestToken();
            var callback = Url.ToPublicUrl(new Uri(URL.f1CalBack, UriKind.Relative));
            return Redirect(string.Format(URL.f1AuthorizeUrl, "hwychrau", requestToken.Value, callback));
        }

        public ActionResult CallBack()
        {
            Load();
            var accessToken = _f1AuthHelper.RequestAccessToken();
            return RedirectToAction("Ready");
        }

        public ActionResult Ready()
        {
            return View();
        }
    }
}
