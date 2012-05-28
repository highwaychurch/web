using Hammock.Authentication.OAuth;

namespace F1PCO.Web.Integration.PCO
{
    public interface IPCOAuthorizationService
    {
        bool TryAuthorizeWithPersistedAccessToken(Token persistedAccessToken);

        bool IsAuthorized { get; }

        Token RequestAndPersistAccessToken();

        string BuildPortalUserAuthorizationRequestUrl(string callbackUrl);

        OAuthCredentials GetAccessTokenCredentials();
    }
}