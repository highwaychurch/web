using System.Threading.Tasks;
using F1PCO.OAuth;

namespace F1PCO.Integration.F1
{
    public interface IF1AuthorizationService
    {
        Task<RequestToken> GetRequestTokenAsync(string callbackUrl);
        Task<AccessToken> GetAccessTokenAsync(RequestToken requestToken);
        Task<bool> TryConnectWithPersistedAccessTokenAsync(AccessToken persistedAccessToken);
        string BuildPortalUserAuthorizationRequestUrl(RequestToken requestToken, string callbackUrl);
    }
}