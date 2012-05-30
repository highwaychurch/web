using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using F1PCO.OAuth;
using RestSharp;
using RestSharp.Authenticators;

namespace F1PCO.Integration.F1
{
    public class F1AuthorizationService : IF1AuthorizationService
    {
        private const string RequestTokenPath = "v1/Tokens/RequestToken";
        private const string AccessTokenPath = "v1/Tokens/AccessToken";
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

        public async Task<bool> TryConnectWithPersistedAccessTokenAsync(AccessToken persistedAccessToken)
        {
            try
            {
                await _testRepository.Value.SearchByNameAsync("TEST", 1);
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
                new RestClient(_apiBaseUrl)
                    {
                        Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret, callbackUrl)
                    };

            var request = new RestRequest(RequestTokenPath);
            var response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var queryString = HttpUtility.ParseQueryString(response.Content);
                var requestToken = new RequestToken(queryString["oauth_token"], queryString["oauth_token_secret"]);
                return requestToken;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
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
                new RestClient(_apiBaseUrl)
                    {
                        Authenticator = OAuth1Authenticator.ForAccessToken(_consumerKey, _consumerSecret, requestToken.Value, requestToken.Secret)
                    };
            var request = new RestRequest(AccessTokenPath);
            var response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var queryString = HttpUtility.ParseQueryString(response.Content);
                var accessToken = new AccessToken(queryString["oauth_token"], queryString["oauth_token_secret"]);
                return accessToken;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
        }
    }
}