namespace F1PCO.OAuth
{
    public class AccessToken : TokenBase
    {
        public AccessToken()
        {
        }

        public AccessToken(string value, string secret) : base(value, secret)
        {
        }
    }
}