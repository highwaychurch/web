using System;
using System.Linq;
using Raven.Client;
using RestSharp;
using RestSharp.Authenticators;

namespace F1PCO.Integration.F1
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

        public IRestClient GetRestClient()
        {
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("There is no current user.");

            var client =
                new RestClient(_apiBaseUrl)
                    {
                        Authenticator = OAuth1Authenticator.ForProtectedResource(_consumerKey, _consumerSecret, user.F1AccessToken.Value, user.F1AccessToken.Secret)
                    };

            return client;
        }
    }
}