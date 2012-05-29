using System;
using System.Linq;
using Raven.Client;
using RestSharp;
using RestSharp.Authenticators;

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
            var user = _session.Query<User>().FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("There is no current user.");

            var client =
                new RestClient(_apiBaseUrl)
                    {
                        Authenticator =
                            OAuth1Authenticator.ForProtectedResource(_consumerKey, _consumerSecret,
                                                                     user.PCOAccessToken.Value,
                                                                     user.PCOAccessToken.Secret)
                    };

            return client;
        }
    }
}