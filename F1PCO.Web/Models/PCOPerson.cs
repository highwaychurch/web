using System;
using System.Collections.Generic;

namespace F1PCO.Web.Models
{
    public class PCOPerson
    {
        public PCOPerson()
        {
            PersonLogs = new List<string>();
        }

        public string PCOID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<string> PersonLogs { get; private set; }
    }
}