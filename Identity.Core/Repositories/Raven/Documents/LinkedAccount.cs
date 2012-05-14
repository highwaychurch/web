namespace Highway.Identity.Core.Repositories.Raven.Documents
{
    public class LinkedAccount
    {
        public string ClaimedIdentifier { get; set; }
        public string ProviderName { get; set; }
        public string ProviderUri { get; set; }
    }
}