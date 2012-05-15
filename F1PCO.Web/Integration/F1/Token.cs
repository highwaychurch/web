namespace F1PCO.Web.Integration.F1
{
    public class Token
    {
        public Token() { }

        public Token(string value, string secret)
        {
            Value = value;
            Secret = secret;
        }

        public string Secret { get; set; }
        public string Value { get; set; }
    }
}