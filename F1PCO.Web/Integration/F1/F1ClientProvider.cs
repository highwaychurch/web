using System;
using System.Linq;
using Hammock;
using Hammock.Authentication.OAuth;
using Raven.Client;

namespace F1PCO.Web.Integration.F1
{
    public class F1ClientProvider : IF1ClientProvider
    {
        private readonly IDocumentSession _session;
        private readonly string _apiBaseUrl;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public F1ClientProvider(IDocumentSession session, string apiBaseUrl, string consumerKey, string consumerSecret)
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
                           VersionPath = "v1",
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
                               Token = user.F1AccessToken.Value,
                               TokenSecret = user.F1AccessToken.Secret
                           };
            }

            return null;
        }
    }
}