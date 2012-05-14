namespace Highway.Identity.Core.Repositories.Raven.Documents
{
    public class EndpointsConfiguration
    {
        public string Id { get; set; }
        public bool WSFederation { get; set; }
        public bool WSTrustMessage { get; set; }
        public bool WSTrustMixed { get; set; }
        public bool SimpleHttp { get; set; }
        public bool FederationMetadata { get; set; }
        public bool OAuthWRAP { get; set; }
        public bool OAuth2 { get; set; }
        public bool JSNotify { get; set; }
        public int HttpPort { get; set; }
        public int HttpsPort { get; set; }
    }
}
