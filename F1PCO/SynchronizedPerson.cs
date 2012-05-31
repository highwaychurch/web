using System;
using System.Collections.Generic;
using F1PCO.Integration.F1;
using F1PCO.Integration.PCO;

namespace F1PCO
{
    public class SynchronizedPerson
    {
        public SynchronizedPerson()
        {
            ChangeLog = new List<SynchronizationLogEntry>();
        }

        public string Id { get; set; }
        public string F1ID { get; set; }
        public string PCOID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public F1Person MostRecentF1Person { get; set; }
        public PCOPerson MostRecentPCOPerson { get; set; }

        public DateTime LastSynchronized { get; set; }

        public List<SynchronizationLogEntry> ChangeLog { get; private set; }
    }

    public class SynchronizationLogEntry
    {
        public DateTime TimeStampUtc { get; set; }
    }
}
