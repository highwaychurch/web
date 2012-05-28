using Hammock;

namespace F1PCO.Web.Integration.PCO
{
    public interface IPCOClientProvider
    {
        IRestClient GetRestClient(string contentType = null);
    }
}