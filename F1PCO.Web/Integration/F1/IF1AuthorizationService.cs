using Hammock.Authentication.OAuth;

namespace F1PCO.Web.Integration.F1
{
    public interface IF1AuthorizationService
    {
        bool IsAuthorized { get; }

        Token RequestAndPersistAccessToken();

        string BuildPortalUserAuthorizationRequestUrl(string callbackUrl);

        OAuthCredentials GetAccessTokenCredentials();
    }
}