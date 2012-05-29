namespace F1PCO.OAuth
{
    public abstract class TokenBase
    {
        protected TokenBase() { }

        protected TokenBase(string value, string secret)
        {
            Value = value;
            Secret = secret;
        }

        public string Secret { get; set; }
        public string Value { get; set; }
    }
}