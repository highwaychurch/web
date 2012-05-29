namespace F1PCO.OAuth
{
    public class RequestToken : TokenBase
    {
        public RequestToken()
        {
        }

        public RequestToken(string value, string secret) : base(value, secret)
        {
        }
    }
}