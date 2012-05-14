/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.ServiceModel;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.SWT;
using Highway.Identity.Core.Tracing;
using Highway.Identity.Web.ActionResults;
using Highway.Identity.Web.Security;
using Microsoft.IdentityModel.Claims;

namespace Highway.Identity.Web.Controllers.Issue
{
    public class WrapController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly AuthenticationHelper _authenticationHelper;

        public WrapController(IConfigurationRepository configurationRepository, AuthenticationHelper authenticationHelper)
        {
            _configurationRepository = configurationRepository;
            _authenticationHelper = authenticationHelper;
        }

        [HttpPost]
        public ActionResult Issue()
        {
            if (!_configurationRepository.Endpoints.OAuthWRAP)
            {
                Tracing.Error("OAuth WRAP endpoint is disabled in configuration");
                return new HttpNotFoundResult();
            }

            var scope = Request.Form["wrap_scope"];

            if (string.IsNullOrWhiteSpace(scope))
            {
                Tracing.Error("OAuth WRAP endpoint called with empty realm.");
                return new HttpStatusCodeResult(400);
            }

            Uri uri;
            if (!Uri.TryCreate(scope, UriKind.Absolute, out uri))
            {
                Tracing.Error("OAuth WRAP endpoint called with malformed realm: " + scope);
                return new HttpStatusCodeResult(400);
            }

            Tracing.Information("OAuth WRAP endpoint called with realm: " + scope);

            var endpoint = new EndpointAddress(uri);

            IClaimsPrincipal principal;
            if (!_authenticationHelper.TryGetPrincipalFromWrapRequest(Request, out principal))
            {
                Tracing.Error("Authentication failed");
                return new UnauthorizedResult("WRAP", UnauthorizedResult.ResponseAction.Send401);
            }

            if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.WRAP))
            {
                Tracing.Error("User not authorized");
                return new UnauthorizedResult("WRAP", UnauthorizedResult.ResponseAction.Send401);
            }

            TokenResponse response;
            if (_authenticationHelper.TryIssueToken(endpoint, principal, SimpleWebToken.OasisTokenProfile, out response))
            {
                return new WrapResult(response);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }
    }
}
