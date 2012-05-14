using System;

namespace Highway.Identity.Core.Models
{
    public class DelegationModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public Uri Realm { get; set; }
        public string Description { get; set; }
    }
}
