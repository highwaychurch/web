namespace Highway.Identity.Core.Repositories.Raven.Documents
{
    public class ClientCertificate
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Thumbprint { get; set; }
        public string Description { get; set; }
    }
}
