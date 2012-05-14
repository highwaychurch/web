namespace Highway.Identity.Core.Repositories.Raven.Documents
{
    public class RelyingParty
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Realm { get; set; }
        public string ReplyTo { get; set; }
        public string EncryptingCertificate { get; set; }
        public string SymmetricSigningKey { get; set; }
        public string ExtraData1 { get; set; }
        public string ExtraData2 { get; set; }
        public string ExtraData3 { get; set; }
    }
}
