using System.Collections.Generic;

namespace Highway.Identity.Core.Models
{
    public class UserAccountModel
    {
        public UserAccountModel()
        {
            LinkedAccounts = new List<LinkedAccountModel>();
            ClientCertificates = new List<ClientCertificateModel>();
        }

        public string Id { get; set; }
        public string UniqueId { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public List<LinkedAccountModel> LinkedAccounts { get; private set; }
        public List<ClientCertificateModel> ClientCertificates { get; private set; } 
    }
}
