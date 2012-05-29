using RestSharp;

namespace F1PCO.Integration.F1
{
    public interface IF1ClientProvider
    {
        IRestClient GetRestClient();
    }
}