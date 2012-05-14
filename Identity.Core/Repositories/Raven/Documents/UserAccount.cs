using System.Collections.Generic;

namespace Highway.Identity.Core.Repositories.Raven.Documents
{
    public class UserAccount
    {
        public UserAccount()
        {
            LinkedAccounts = new List<LinkedAccount>();
            ClientCertificates = new List<ClientCertificate>();
        }

        public string Id { get; set; }
        public string UniqueId { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public List<LinkedAccount> LinkedAccounts { get; private set; }
        public List<ClientCertificate> ClientCertificates { get; private set; }
    }
}
