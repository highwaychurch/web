using RestSharp;

namespace F1PCO.Integration.PCO
{
    public interface IPCOClientProvider
    {
        IRestClient GetRestClient(string contentType = null);
    }
}