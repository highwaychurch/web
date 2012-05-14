using System.Security.Cryptography.X509Certificates;

namespace Highway.Identity.Core.Models
{
    public class CertificateModel
    {
        public string Id { get; set; }
        public string SubjectDistinguishedName { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }
}
