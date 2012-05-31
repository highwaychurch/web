using System;

namespace F1PCO.Integration.PCO
{
    public class PCOPerson
    {
        public string PCOID { get; set; }
        public DateTime LastUpdatedAtUtc { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
    }
}