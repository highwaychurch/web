using System.Threading.Tasks;
using F1PCO.OAuth;

namespace F1PCO.Integration.PCO
{
    public interface IPCOAuthorizationService
    {
        Task<RequestToken> GetRequestTokenAsync(string callbackUrl);
        Task<AccessToken> GetAccessTokenAsync(RequestToken requestToken, string verifier);
        Task<bool> TryConnectWithPersistedAccessTokenAsync(AccessToken persistedAccessToken);
        string BuildAuthorizationRequestUrl(RequestToken requestToken, string callbackUrl);
    }
}