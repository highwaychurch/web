using System;
using System.Web;
using Hammock;
using Hammock.Authentication.OAuth;
using Highway.Shared.Mvc;

namespace F1PCO.Web.Integration.F1
{
    public class F1AuthorizationService : IF1AuthorizationService
    {
        private Token _requestToken;
        private Token _accessToken;
        private readonly HttpResponseBase _response;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _apiBaseUrl;
        private readonly string _portalUserAuthorizeUrlFormat;

        public F1AuthorizationService(HttpRequestBase request, HttpResponseBase response, string consumerKey, string consumerSecret, string apiBaseUrl, string portalUserAuthorizeUrlFormat)
        {
            _response = response;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _apiBaseUrl = apiBaseUrl;
            _portalUserAuthorizeUrlFormat = portalUserAuthorizeUrlFormat;

            request.Cookies.TryGetFromCookie("F1RequestToken", out _requestToken);
            request.Cookies.TryGetFromCookie("F1AccessToken", out _accessToken);
        }

        public bool IsAuthorized
        {
            get { return AccessToken != null; }
        }

        public Token AccessToken
        {
            get { return _accessToken; }
        }

        public string BuildPortalUserAuthorizationRequestUrl(string callbackUrl)
        {
            GetRequestToken();

            return _portalUserAuthorizeUrlFormat
                .Replace("{oauth_token}", _requestToken.Value)
                .Replace("{oauth_callback}", callbackUrl);
        }

        public OAuthCredentials GetAccessTokenCredentials()
        {
            return new OAuthCredentials
                       {
                           Type = OAuthType.AccessToken,
                           SignatureMethod = OAuthSignatureMethod.HmacSha1,
                           ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                           ConsumerKey = _consumerKey,
                           ConsumerSecret = _consumerSecret,
                           Token = AccessToken.Value,
                           TokenSecret = AccessToken.Secret
                       };
        }

        public Token GetRequestToken()
        {
            var client =
                new RestClient
                    {
                        Authority = _apiBaseUrl,
                        VersionPath = "v1",
                        Credentials =
                            new OAuthCredentials
                                {
                                    Type = OAuthType.RequestToken,
                                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                                    ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                                    ConsumerKey = _consumerKey,
                                    ConsumerSecret = _consumerSecret
                                }
                    };

            var request = new RestRequest
            {
                Path = "Tokens/RequestToken"
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _requestToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie("F1RequestToken", _requestToken);
            return _requestToken;
        }

        public Token RequestAndPersistAccessToken()
        {
            if (_requestToken == null) throw new InvalidOperationException("Cannot request an Access token until you have requested a Request token.");

            var client =
                new RestClient
                    {
                        Authority = _apiBaseUrl,
                        VersionPath = "v1",
                        Credentials =
                            new OAuthCredentials
                                {
                                    Type = OAuthType.AccessToken,
                                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                                    ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                                    ConsumerKey = _consumerKey,
                                    ConsumerSecret = _consumerSecret,
                                    Token = _requestToken.Value,
                                    TokenSecret = _requestToken.Secret
                                }
                    };

            var request = new RestRequest
            {
                Path = "Tokens/AccessToken"
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _accessToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie("F1AccessToken", _accessToken);
            return _accessToken;
        }
    }
}