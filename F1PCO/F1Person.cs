using System;
using System.Collections.Generic;

namespace F1PCO
{
    public class F1Person
    {
        public F1Person()
        {
            PersonLogs = new List<string>();
        }

        public string F1ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<string> PersonLogs { get; private set; }
    }
}