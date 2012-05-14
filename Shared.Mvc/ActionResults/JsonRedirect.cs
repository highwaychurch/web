namespace Highway.Shared.Mvc.ActionResults
{
    public class JsonRedirect
    {
        readonly string _redirectUrl;

        public JsonRedirect(string redirectUrl)
        {
            _redirectUrl = redirectUrl;
        }

        public string RedirectUrl
        {
            get { return _redirectUrl; }
        }
    }
}