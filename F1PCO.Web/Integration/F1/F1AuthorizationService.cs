using System;
using System.Net;
using System.Web;
using Hammock;
using Hammock.Authentication.OAuth;
using Highway.Shared.Mvc;

namespace F1PCO.Web.Integration.F1
{
    public class F1AuthorizationService : IF1AuthorizationService
    {
        private const string AccessTokenCookieKey = "F1AccessToken";
        private const string RequestTokenCookieKey = "F1RequestToken";
        private const string RequestTokenPath = "Tokens/RequestToken";
        private const string AccessTokenPath = "Tokens/AccessToken";
        private const string PortalUserAuthorizePath = "v1/PortalUser/Login";
        private Token _requestToken;
        private Token _accessToken;
        private readonly HttpResponseBase _response;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _apiBaseUrl;
        private readonly Lazy<IF1PersonRepository> _testRepository;

        public F1AuthorizationService(HttpRequestBase request, HttpResponseBase response, string consumerKey, string consumerSecret, string apiBaseUrl, Lazy<IF1PersonRepository> testRepository)
        {
            _response = response;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _apiBaseUrl = apiBaseUrl;
            _testRepository = testRepository;

            request.Cookies.TryGetFromCookie(RequestTokenCookieKey, out _requestToken);
            request.Cookies.TryGetFromCookie(AccessTokenCookieKey, out _accessToken);
        }

        public bool TryAuthorizeWithPersistedAccessToken(Token persistedAccessToken)
        {
            _accessToken = persistedAccessToken;
            _response.Cookies.SaveToCookie(AccessTokenCookieKey, persistedAccessToken);

            try
            {
                _testRepository.Value.SearchByName("TEST", 1);
                return true;
            }
            catch
            {
                return false;
            }
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
            builder.Path = builder.Path.TrimEnd('/') + "/" + PortalUserAuthorizePath.TrimStart('/');
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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var queryString = HttpUtility.ParseQueryString(response.Content);
                _accessToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
                _response.Cookies.SaveToCookie(AccessTokenCookieKey, _accessToken);
                return _accessToken;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
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
            _response.Cookies.SaveToCookie(AccessTokenCookieKey, _accessToken);
            return _accessToken;
        }
    }
}