namespace Highway.Identity.Core.Models
{
    public class GlobalConfigurationModel
    {
        public string Id { get; set; }
        public string SiteName { get; set; }
        public string IssuerUri { get; set; }
        public string IssuerContactEmail { get; set; }
        public string DefaultTokenType { get; set; }
        public int DefaultTokenLifetime { get; set; }
        public int MaximumTokenLifetime { get; set; }
        public int SsoCookieLifetime { get; set; }
        public bool RequireSsl { get; set; }
        public bool RequireEncryption { get; set; }
        public bool RequireSignInConfirmation { get; set; }
        public bool RequireReplyToWithinRealm { get; set; }
        public bool AllowKnownRealmsOnly { get; set; }
        public bool AllowReplyTo { get; set; }
        public bool EnableClientCertificates { get; set; }
        public bool EnableDelegation { get; set; }
        public bool EnableStrongEpiForSsl { get; set; }
        public bool EnableFederationMessageTracing { get; set; }
        public bool EnforceUsersGroupMembership { get; set; }
    }
}


