using F1PCO.OAuth;

namespace F1PCO.Integration.PCO
{
    public interface IPCOAuthorizationService
    {
        RequestToken GetRequestToken(string callbackUrl);
        AccessToken GetAccessToken(RequestToken requestToken, string verifier);
        bool TryConnectWithPersistedAccessToken(AccessToken persistedAccessToken);
        string BuildAuthorizationRequestUrl(RequestToken requestToken, string callbackUrl);
    }
}