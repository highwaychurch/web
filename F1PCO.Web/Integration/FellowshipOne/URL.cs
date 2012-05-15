namespace F1OAuthTest.Integration.FellowshipOne
{
    public static class URL
    {
        //Staging
        public const string F1BaseUrl = "https://{0}.staging.fellowshiponeapi.com";
        public const string f1AuthorizeUrl = "https://{0}.staging.fellowshiponeapi.com/v1/PortalUser/Login?oauth_token={1}&oauth_callback={2}";

        //Prod
        //public const string F1BaseUrl = "https://{0}.fellowshiponeapi.com";
        //public const string f1AuthorizeUrl = "https://{0}.fellowshiponeapi.com/v1/PortalUser/Login?oauth_token={1}&oauth_callback={2}";
        public const string f1CalBack = "CallBack?provider=f1";
    }
}