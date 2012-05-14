using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Highway.Shared.Mvc.Security;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Web;

namespace Highway.Profiles.Web.Controllers
{
    //public class TokenController : Controller
    //{
    //    //
    //    // GET: /Token/ProcessRequest

    //    public ActionResult ProcessRequest()
    //    {
    //        string action = Request.QueryString[WSFederationConstants.Parameters.Action];

    //        switch (action)
    //        {
    //            case WSFederationConstants.Actions.SignIn:
    //                var validationActionResult = ValidateTokenRequest();
    //                if (validationActionResult != null) return validationActionResult;
    //                return SignIn();

    //            case WSFederationConstants.Actions.SignOut:
    //                return SignOut();
    //        }

    //        return null;
    //    }

    //    private ActionResult SignIn()
    //    {
    //        var message = WSFederationMessage.CreateFromUri(Request.Url);
    //        var signInMessage = message as SignInRequestMessage;

    //        if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
    //        {
    //            // The user isn't known at this point so we need to authenticate them
    //            return new HttpUnauthorizedResult();
    //        }

    //        var claims = new List<Claim>
    //        {
    //            new Claim(WSIdentityConstants.ClaimTypes.Name, User.Identity.Name),
    //            new Claim(ClaimTypes.AuthenticationMethod, FormsAuthenticationHelper.GetAuthenticationMethod(User.Identity))
    //        };

    //        var identity = new ClaimsIdentity(claims, AuthenticationTypes.Password);
    //        var principal = ClaimsPrincipal.CreateFromIdentity(identity);
    //        FederatedPassiveSecurityTokenServiceOperations.ProcessRequest(
    //            System.Web.HttpContext.Current.Request,
    //            principal,
    //            StarterTokenServiceConfiguration.Current.CreateSecurityTokenService(),
    //            System.Web.HttpContext.Current.Response);

    //        return null;
    //    }

    //    private ActionResult ValidateTokenRequest()
    //    {
    //        string realm = Request.QueryString[WSFederationConstants.Parameters.Realm];
    //        if (STS.Configuration.AllowKnownRealmsOnly)
    //        {
    //            // check if the relying party is registered
    //            RelyingParty rp = null;
    //            if (RelyingPartyEntity.TryGet(realm, out rp))
    //            {
    //                // The realm is registered so that's great
    //                if (STS.Configuration.RequireReplyToWithinRealm)
    //                {
    //                    var replyTo = Request.QueryString[WSFederationConstants.Parameters.Reply];
    //                    if (!string.IsNullOrEmpty(replyTo))
    //                    {
    //                        // Ensure the the reply to address is within the realm
    //                        if (replyTo.StartsWith(realm) == false)
    //                        {
    //                            return Error("The ReplyTo address must be within the Realm.");
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                return Error(string.Format("The Realm is not known: '{0}'. In order to use this Identity Provider you must register the realm as a trusted Relying Party.", realm));
    //            }
    //        }

    //        return null;
    //    }

    //    private ActionResult SignOut()
    //    {
    //        var requestMessage = (SignOutRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);

    //        if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
    //        {
    //            throw new UnauthorizedAccessException();
    //        }

    //        FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(requestMessage, User, requestMessage.Reply, System.Web.HttpContext.Current.Response);
    //        return null;
    //    }

    //    public ActionResult Error(string errorMessage)
    //    {
    //        ViewBag.ErrorMessage = errorMessage;
    //        return View("Error");
    //    }
    //}
}
