using System;
using System.Net;
using System.Web;
using Hammock;
using Hammock.Authentication.OAuth;
using Highway.Shared.Mvc;

namespace F1PCO.Web.Integration.PCO
{
    public class PCOAuthorizationService : IPCOAuthorizationService
    {
        private const string AccessTokenCookieKey = "PCOAccessToken";
        private const string RequestTokenCookieKey = "PCORequestToken";
        private const string RequestTokenPath = "oauth/request_token";
        private const string AccessTokenPath = "oauth/access_token";
        private const string PortalUserAuthorizePath = "oauth/authorize";
        private Token _requestToken;
        private Token _accessToken;
        private readonly HttpRequestBase _request;
        private readonly HttpResponseBase _response;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _apiBaseUrl;
        private readonly Lazy<IPCOPersonRepository> _testRepository;

        public PCOAuthorizationService(HttpRequestBase request, HttpResponseBase response, string consumerKey, string consumerSecret, string apiBaseUrl, Lazy<IPCOPersonRepository> testRepository)
        {
            _request = request;
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
                _testRepository.Value.GetPeople();
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
            GetRequestToken(callbackUrl);

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

        public Token GetRequestToken(string callbackUrl)
        {
            var client =
                new RestClient
                    {
                        Authority = _apiBaseUrl,
                        Credentials =
                            new OAuthCredentials
                                {
                                    Type = OAuthType.RequestToken,
                                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                                    ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                                    ConsumerKey = _consumerKey,
                                    ConsumerSecret = _consumerSecret,
                                    CallbackUrl = callbackUrl
                                }
                    };

            var request = new RestRequest
            {
                Path = RequestTokenPath
            };

            var response = client.Request(request);

            var queryString = HttpUtility.ParseQueryString(response.Content);
            _requestToken = new Token(queryString["oauth_token"], queryString["oauth_token_secret"]);
            _response.Cookies.SaveToCookie(RequestTokenCookieKey, _requestToken);
            return _requestToken;
        }

        public Token RequestAndPersistAccessToken()
        {
            if (_requestToken == null) throw new InvalidOperationException("Cannot request an Access token until you have requested a Request token.");

            var verifier = _request.QueryString["oauth_verifier"];
            if (string.IsNullOrWhiteSpace(verifier))
                throw new Exception("There was no oauth_verifier parameter on the callback request.");

            var client =
                new RestClient
                    {
                        Authority = _apiBaseUrl,
                        Credentials =
                            new OAuthCredentials
                                {
                                    Type = OAuthType.AccessToken,
                                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                                    ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                                    ConsumerKey = _consumerKey,
                                    ConsumerSecret = _consumerSecret,
                                    Token = _requestToken.Value,
                                    TokenSecret = _requestToken.Secret,
                                    Verifier = verifier
                                }
                    };

            var request = new RestRequest
            {
                Path = AccessTokenPath
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
    }
}