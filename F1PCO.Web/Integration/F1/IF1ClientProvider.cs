using Hammock;

namespace F1PCO.Web.Integration.F1
{
    public interface IF1ClientProvider
    {
        IRestClient GetRestClient(string contentType = null);
    }
}