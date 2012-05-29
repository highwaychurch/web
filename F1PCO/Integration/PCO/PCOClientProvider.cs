using System;
using System.Linq;
using Hammock;
using Hammock.Authentication.OAuth;
using Raven.Client;

namespace F1PCO.Integration.PCO
{
    public class PCOClientProvider : IPCOClientProvider
    {
        private readonly IDocumentSession _session;
        private readonly string _apiBaseUrl;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public PCOClientProvider(IDocumentSession session, string apiBaseUrl, string consumerKey, string consumerSecret)
        {
            _session = session;
            _apiBaseUrl = apiBaseUrl;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        public IRestClient GetRestClient(string contentType = null)
        {
            var client =
                new RestClient
                       {
                           Authority = _apiBaseUrl,
                           Credentials = GetAccessTokenCredentials(),
                       };
            if (string.IsNullOrWhiteSpace(contentType) == false)
            {
                client.AddHeader("content-type", contentType);
            }

            return client;
        }

        public OAuthCredentials GetAccessTokenCredentials()
        {
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("There is no current user.");

            if (user.F1AccessToken != null)
            {
                return new OAuthCredentials
                           {
                               Type = OAuthType.AccessToken,
                               SignatureMethod = OAuthSignatureMethod.HmacSha1,
                               ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                               ConsumerKey = _consumerKey,
                               ConsumerSecret = _consumerSecret,
                               Token = user.PCOAccessToken.Value,
                               TokenSecret = user.PCOAccessToken.Secret
                           };
            }

            return null;
        }
    }
}