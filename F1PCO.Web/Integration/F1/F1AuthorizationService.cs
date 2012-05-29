using System;
using System.Net;
using System.Web;
using F1PCO.OAuth;
using Hammock;
using Hammock.Authentication.OAuth;

namespace F1PCO.Web.Integration.F1
{
    public class F1AuthorizationService : IF1AuthorizationService
    {
        private const string RequestTokenPath = "Tokens/RequestToken";
        private const string AccessTokenPath = "Tokens/AccessToken";
        private const string PortalUserAuthorizePath = "v1/PortalUser/Login";
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _apiBaseUrl;
        private readonly Lazy<IF1PersonRepository> _testRepository;

        public F1AuthorizationService(string consumerKey, string consumerSecret, string apiBaseUrl, Lazy<IF1PersonRepository> testRepository)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _apiBaseUrl = apiBaseUrl;
            _testRepository = testRepository;
        }

        public bool TryConnectWithPersistedAccessToken(AccessToken persistedAccessToken)
        {
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

        public RequestToken GetRequestToken(string callbackUrl)
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
                                    ConsumerSecret = _consumerSecret,
                                    CallbackUrl = callbackUrl
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
                var requestToken = new RequestToken(queryString["oauth_token"], queryString["oauth_token_secret"]);
                return requestToken;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
        }

        public string BuildPortalUserAuthorizationRequestUrl(RequestToken requestToken, string callbackUrl)
        {
            var builder = new UriBuilder(_apiBaseUrl);
            builder.Path = builder.Path.TrimEnd('/') + "/" + PortalUserAuthorizePath.TrimStart('/');
            builder.Query = string.Format("oauth_token={0}&oauth_callback={1}", requestToken.Value, callbackUrl);
            return builder.ToString();
        }

        public AccessToken GetAccessToken(RequestToken requestToken)
        {
            if (requestToken == null) throw new InvalidOperationException("Cannot get an Access token until you have the Request token.");

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
                                    Token = requestToken.Value,
                                    TokenSecret = requestToken.Secret
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
                var accessToken = new AccessToken(queryString["oauth_token"], queryString["oauth_token_secret"]);
                return accessToken;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
        }
    }
}