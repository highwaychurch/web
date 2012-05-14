using System;
using System.ServiceModel;
using System.Threading;
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
    [ClaimsAuthorize(Constants.Actions.Issue, Constants.Resources.JSNotify)]
    public class JSNotifyController : Controller
    {
        readonly IConfigurationRepository _configurationRepository;
        readonly AuthenticationHelper _authenticationHelper;

        public JSNotifyController(IConfigurationRepository configurationRepository, AuthenticationHelper authenticationHelper)
        {
            _configurationRepository = configurationRepository;
            _authenticationHelper = authenticationHelper;
        }

        public ActionResult Issue(string realm, string tokenType)
        {
            if (!_configurationRepository.Endpoints.JSNotify)
            {
                Tracing.Warning("JSNotify endpoint called, but disabled in configuration");
                return new HttpNotFoundResult();
            }

            Tracing.Information("JSNotify endpoint called for realm: " + realm);

            if (tokenType == null)
            {
                tokenType = SimpleWebToken.OasisTokenProfile;
            }

            Tracing.Information("Token type: " + tokenType);

            Uri uri;
            if (!Uri.TryCreate(realm, UriKind.Absolute, out uri))
            {
                Tracing.Error("Realm parameter is malformed.");
                return new HttpStatusCodeResult(400);
            }

            var endpoint = new EndpointAddress(uri);

            TokenResponse response;
            if (_authenticationHelper.TryIssueToken(endpoint, Thread.CurrentPrincipal as IClaimsPrincipal, tokenType, out response))
            {
                var jsresponse = new AccessTokenResponse
                {
                    AccessToken = response.TokenString,
                    TokenType = response.TokenType,
                    ExpiresIn = _configurationRepository.Configuration.DefaultTokenLifetime * 60
                };

                Tracing.Information("JSNotify issue successful for user: " + User.Identity.Name);
                return new JSNotifyResult(jsresponse);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }
    }
}
