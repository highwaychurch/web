using System;
using System.Web;
using Hammock;
using Hammock.Authentication.OAuth;
using Highway.Shared.Mvc;

namespace F1PCO.Web.Integration.F1
{
    public class F1AuthorizationService : IF1AuthorizationService
    {
        private const string F1AccessTokenCookieKey = "F1AccessToken";
        private const string F1RequestTokenCookieKey = "F1RequestToken";
        private const string RequestTokenPath = "Tokens/RequestToken";
        private const string AccessTokenPath = "Tokens/AccessToken";
        private const string PortalUserAuthorizePath = "v1/PortalUser/Login";
        private Token _requestToken;
        private Token _accessToken;
        private readonly HttpResponseBase _response;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _apiBaseUrl;

        public F1AuthorizationService(HttpRequestBase request, HttpResponseBase response, string consumerKey, string consumerSecret, string apiBaseUrl)
        {
            _response = response;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _apiBaseUrl = apiBaseUrl;

            request.Cookies.TryGetFromCookie(F1RequestTokenCookieKey, out _requestToken);
            request.Cookies.TryGetFromCookie(F1AccessTokenCookieKey, out _accessToken);
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

            var builder = new UriBuilder(_apiBaseUrl);
            builder.Path += "/" + PortalUserAuthorizePath.TrimStart('/');
            builder.Query = string.Format("oauth_token={0}&oauth_callback={1}", _requestToken.Value, callbackUrl);
            return builder.ToString();
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
                Path = RequestTokenPath
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _requestToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie(F1RequestTokenCookieKey, _requestToken);
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
                Path = AccessTokenPath
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _accessToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie(F1AccessTokenCookieKey, _accessToken);
            return _accessToken;
        }
    }
}