using Hammock;

namespace F1PCO.Web.Integration.F1
{
    public class F1ClientProvider : IF1ClientProvider
    {
        private readonly IF1AuthorizationService _authorizationService;
        private readonly string _apiBaseUrl;

        public F1ClientProvider(IF1AuthorizationService authorizationService, string apiBaseUrl)
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
                           VersionPath = "v1",
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