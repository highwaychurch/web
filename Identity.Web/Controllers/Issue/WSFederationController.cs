/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Security.Principal;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.TokenService;
using Highway.Identity.Web.ActionResults;
using Highway.Identity.Web.Security;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Web;

namespace Highway.Identity.Web.Controllers.Issue
{
    [ClaimsAuthorize(Constants.Actions.Issue, Constants.Resources.WSFederation)]
    public class WSFederationController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;

        public WSFederationController(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public ActionResult Issue()
        {
            if (!_configurationRepository.Endpoints.WSFederation)
            {
                return new HttpNotFoundResult();
            }

            var message = WSFederationMessage.CreateFromUri(HttpContext.Request.Url);

            // sign in 
            var signinMessage = message as SignInRequestMessage;
            if (signinMessage != null)
            {
                return ProcessWSFederationSignIn(signinMessage, HttpContext.User);
            }

            // sign out
            var signoutMessage = message as SignOutRequestMessage;
            if (signoutMessage != null)
            {
                return ProcessWSFederationSignOut(signoutMessage);
            }

            return View("Error");
        }

        #region Helper
        private ActionResult ProcessWSFederationSignIn(SignInRequestMessage message, IPrincipal principal)
        {
            // issue token and create ws-fed response
            var response = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(
                message,
                principal,
                TokenServiceConfiguration.Current.CreateSecurityTokenService());

            // set cookie for single-sign-out
            new SignInSessionsManager(HttpContext, _configurationRepository.Configuration.MaximumTokenLifetime)
                .AddRealm(response.BaseUri.AbsoluteUri);

            return new WSFederationResult(response);
        }

        private ActionResult ProcessWSFederationSignOut(SignOutRequestMessage message)
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            // check for return url
            if (!string.IsNullOrWhiteSpace(message.Reply))
            {
                ViewBag.ReturnUrl = message.Reply;
            }

            // check for existing sign in sessions
            var mgr = new SignInSessionsManager(HttpContext);
            var realms = mgr.GetRealms();
            mgr.ClearRealms();
            
            return View("Signout", realms);
        }
        #endregion
    }
}
