using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Web.Mvc;
using Highway.Identity.Core;
using Highway.Identity.Core.Repositories;
using Highway.Identity.Core.SWT;
using Highway.Identity.Core.Tracing;
using Highway.Identity.Web.ActionResults;
using Highway.Identity.Web.Security;
using Highway.Identity.Web.ViewModels.OAuth2;
using Microsoft.IdentityModel.Claims;

namespace Highway.Identity.Web.Controllers.Issue
{
    public class OAuth2Controller : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly AuthenticationHelper _authenticationHelper;

        public OAuth2Controller(IConfigurationRepository configurationRepository, AuthenticationHelper authenticationHelper)
        {
            _configurationRepository = configurationRepository;
            _authenticationHelper = authenticationHelper;
        }

        [HttpPost]
        public ActionResult Token(ResourceOwnerCredentialRequest request)
        {
            if (!_configurationRepository.Endpoints.OAuth2)
            {
                Tracing.Error("OAuth2 endpoint called, but disabled in configuration");
                return new HttpNotFoundResult();
            }

            if (!ModelState.IsValid)
            {
                Tracing.Error("OAuth2 called with malformed request");
                return new HttpStatusCodeResult(400);
            }

            Uri uri;
            if (!Uri.TryCreate(request.Scope, UriKind.Absolute, out uri))
            {
                Tracing.Error("OAuth2 endpoint called with malformed realm: " + request.Scope);
                return new HttpStatusCodeResult(400);
            }

            IClaimsPrincipal principal = null;
            if (_authenticationHelper.TryGetPrincipalFromOAuth2Request(Request, request, out principal))
            {
                if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.OAuth2))
                {
                    Tracing.Error("User not authorized");
                    return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);
                }

                SecurityToken token;
                if (_authenticationHelper.TryIssueToken(new EndpointAddress(uri), principal, SimpleWebToken.OasisTokenProfile, out token))
                {
                    var swt = token as SimpleWebToken;
                    var response = new AccessTokenResponse
                    {
                        AccessToken = swt.RawToken,
                        TokenType = SimpleWebToken.OasisTokenProfile,
                        ExpiresIn = _configurationRepository.Configuration.DefaultTokenLifetime * 60,
                    };

                    Tracing.Information("OAuth2 issue successful for user: " + request.UserName);
                    return new OAuth2AccessTokenResult(response);
                }

                return new HttpStatusCodeResult(400);
            }

            Tracing.Error("OAuth2 endpoint authentication failed for user: " + request.UserName);
            return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);

            //if (UserRepository.ValidateUser(request.UserName ?? "", request.Password ?? ""))
            //{
            //    var principal = auth.CreatePrincipal(request.UserName, AuthenticationMethods.Password);

            //    if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.OAuth2))
            //    {
            //        Tracing.Error("User not authorized");
            //        return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);
            //    }

            //    SecurityToken token;
            //    if (auth.TryIssueToken(new EndpointAddress(uri), principal, SimpleWebToken.OasisTokenProfile, out token))
            //    {
            //        var swt = token as SimpleWebToken;
            //        var response = new AccessTokenResponse
            //        {
            //            AccessToken = swt.RawToken,
            //            TokenType = SimpleWebToken.OasisTokenProfile,
            //            ExpiresIn = ConfigurationRepository.Configuration.DefaultTokenLifetime * 60,
            //        };

            //        Tracing.Information("OAuth2 issue successful for user: " + request.UserName);
            //        return new OAuth2AccessTokenResult(response);
            //    }

            //    return new HttpStatusCodeResult(400);
            //}

            //Tracing.Error("OAuth2 endpoint authentication failed for user: " + request.UserName);
            //return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);
        }
    }
}
