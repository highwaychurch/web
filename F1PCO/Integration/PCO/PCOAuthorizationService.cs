using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using F1PCO.OAuth;
using RestSharp;
using RestSharp.Authenticators;

namespace F1PCO.Integration.PCO
{
    public class PCOAuthorizationService : IPCOAuthorizationService
    {
        private const string RequestTokenPath = "oauth/request_token";
        private const string AccessTokenPath = "oauth/access_token";
        private const string PortalUserAuthorizePath = "oauth/authorize";
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _apiBaseUrl;
        private readonly Lazy<IPCOPersonRepository> _testRepository;

        public PCOAuthorizationService(string consumerKey, string consumerSecret, string apiBaseUrl, Lazy<IPCOPersonRepository> testRepository)
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
                await _testRepository.Value.SearchByNameAsync("TEST");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string BuildAuthorizationRequestUrl(RequestToken requestToken, string callbackUrl)
        {
            var builder = new UriBuilder(_apiBaseUrl);
            builder.Path = builder.Path.TrimEnd('/') + "/" + PortalUserAuthorizePath.TrimStart('/');
            builder.Query = string.Format("oauth_token={0}&oauth_callback={1}", requestToken.Value, callbackUrl);
            return builder.ToString();
        }

        public async Task<RequestToken> GetRequestTokenAsync(string callbackUrl)
        {
            var client =
                new RestClient(_apiBaseUrl)
                    {
                        Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret, callbackUrl)
                    };
            var request = new RestRequest(RequestTokenPath);
            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var queryString = HttpUtility.ParseQueryString(response.Content);
                var requestToken = new RequestToken(queryString["oauth_token"], queryString["oauth_token_secret"]);
                return requestToken;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
        }

        public async Task<AccessToken> GetAccessTokenAsync(RequestToken requestToken, string verifier)
        {
            if (requestToken == null) throw new InvalidOperationException("Cannot get the Access token without a Request token.");
            if (verifier == null) throw new InvalidOperationException("Cannot get the Access token without a verifer");

            if (string.IsNullOrWhiteSpace(verifier))
                throw new Exception("There was no oauth_verifier parameter on the callback request.");

            var client =
                new RestClient(_apiBaseUrl)
                {
                    Authenticator = OAuth1Authenticator.ForAccessToken(_consumerKey, _consumerSecret, requestToken.Value, requestToken.Secret, verifier)
                };
            var request = new RestRequest(AccessTokenPath);
            var response = await client.ExecuteAsync(request);

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