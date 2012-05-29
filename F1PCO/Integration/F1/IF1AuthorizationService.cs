using F1PCO.OAuth;

namespace F1PCO.Integration.F1
{
    public interface IF1AuthorizationService
    {
        RequestToken GetRequestToken(string callbackUrl);
        AccessToken GetAccessToken(RequestToken requestToken);
        bool TryConnectWithPersistedAccessToken(AccessToken persistedAccessToken);
        string BuildPortalUserAuthorizationRequestUrl(RequestToken requestToken, string callbackUrl);
    }
}