using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using Highway.Shared.Mvc;
using Highway.Shared.Mvc.FlashMessages;

namespace Highway.Identity.Web.Controllers.Integration
{
    public class OpenIdController : Controller
    {
        [HttpPost]
        public ActionResult UseOpenId(string openid_identifier)
        {
            return MakeRequest(openid_identifier);
        }

        public ActionResult UseGoogle()
        {
            return MakeRequest("https://www.google.com/accounts/o8/id");
        }

        private ActionResult MakeRequest(string loginIdentifier)
        {
            if (!Identifier.IsValid(loginIdentifier))
            {
                TempData.FlashError("The loginIdentifier is not valid.");
                return RedirectToAction("SignIn", "Authentication");
            }
            else
            {
                var openid = new OpenIdRelyingParty();

                var replyTo = Url.Action("CompleteRequest", "OpenId", null, Request.Url.Scheme);
                var replyToUri = new Uri(replyTo, UriKind.Absolute);
                var request = openid.CreateRequest(
                    Identifier.Parse(loginIdentifier), Realm.AutoDetect,
                    replyToUri);

                // Require some additional data
                request.AddExtension(new ClaimsRequest
                {
                    BirthDate = DemandLevel.NoRequest,
                    Email = DemandLevel.Require,
                    FullName = DemandLevel.Require
                });

                return request.RedirectingResponse.AsActionResult();
            }
        }

        public ActionResult CompleteRequest()
        {
            var openid = new OpenIdRelyingParty();
            var response = openid.GetResponse();

            if (response != null)
            {
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        var claimedIdentifier = response.ClaimedIdentifier;
                        Debugger.Break();
                        break;
                    case AuthenticationStatus.Canceled:
                        TempData.FlashInfo("Login was cancelled at the provider");
                        break;
                    case AuthenticationStatus.Failed:
                        TempData.FlashInfo("Login failed using the provided OpenID identifier");
                        break;
                }
            }

            return RedirectToAction("SignIn", "Authentication");
        }
    }
}
