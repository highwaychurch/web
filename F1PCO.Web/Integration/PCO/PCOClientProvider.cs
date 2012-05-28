using Hammock;

namespace F1PCO.Web.Integration.PCO
{
    public class PCOClientProvider : IPCOClientProvider
    {
        private readonly IPCOAuthorizationService _authorizationService;
        private readonly string _apiBaseUrl;

        public PCOClientProvider(IPCOAuthorizationService authorizationService, string apiBaseUrl)
        {
            _authorizationService = authorizationService;
            _apiBaseUrl = apiBaseUrl;
        }

        public IRestClient GetRestClient(string contentType = null)
        {
            var client =
                new RestClient
                       {
                           Authority = _apiBaseUrl,
                           Credentials = _authorizationService.GetAccessTokenCredentials(),
                       };
            if (string.IsNullOrWhiteSpace(contentType) == false)
            {
                client.AddHeader("content-type", contentType);
            }

            return client;
        }
    }
}