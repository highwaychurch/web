namespace Highway.Identity.Core.Models
{
    public class LinkedAccountModel
    {
        public string ClaimedIdentifier { get; set; }
        public string ProviderName { get; set; }
        public string ProviderUri { get; set; }
    }
}