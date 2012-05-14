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
using Highway.Identity.Core.Tracing;
using Highway.Identity.Web.ActionResults;
using Highway.Identity.Web.Security;
using Microsoft.IdentityModel.Claims;

namespace Highway.Identity.Web.Controllers.Issue
{
    public class SimpleHttpController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly AuthenticationHelper _authenticationHelper;

        public SimpleHttpController(IConfigurationRepository configurationRepository, AuthenticationHelper authenticationHelper)
        {
            _configurationRepository = configurationRepository;
            _authenticationHelper = authenticationHelper;
        }

        public ActionResult Issue(string realm, string tokenType)
        {
            Tracing.Information("Simple HTTP endpoint called for realm: " + realm);

            if (!_configurationRepository.Endpoints.SimpleHttp)
            {
                Tracing.Warning("Simple HTTP endpoint is disabled in configuration");
                return new HttpNotFoundResult();
            }

            if (tokenType == null)
            {
                tokenType = _configurationRepository.Configuration.DefaultTokenType;
            }

            Tracing.Information("Token type: " + tokenType);

            Uri uri;
            if (!Uri.TryCreate(realm, UriKind.Absolute, out uri))
            {
                Tracing.Error("Realm parameter is malformed.");
                return new HttpStatusCodeResult(400);
            }

            var endpoint = new EndpointAddress(uri);
            
            IClaimsPrincipal principal;
            if (!_authenticationHelper.TryGetPrincipalFromHttpRequest(Request, out principal))
            {
                Tracing.Error("no or invalid credentials found.");
                return new UnauthorizedResult("Basic",  UnauthorizedResult.ResponseAction.Send401);
            }

            if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.SimpleHttp))
            {
                Tracing.Error("User not authorized");
                return new UnauthorizedResult("Basic", UnauthorizedResult.ResponseAction.Send401);
            }

            TokenResponse tokenResponse;
            if (_authenticationHelper.TryIssueToken(endpoint, principal, tokenType, out tokenResponse))
            {
                return new SimpleHttpResult(tokenResponse.TokenString, tokenType);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }        
    }
}
