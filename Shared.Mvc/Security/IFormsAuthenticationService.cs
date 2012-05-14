namespace Highway.Shared.Mvc.Security
{
    public interface IFormsAuthenticationService
    {
        bool IsAuthenticated { get; }
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
        string GetRedirectUrl(string userName, bool createPersistentCookie);
    }
}