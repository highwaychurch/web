using System;
using System.Collections.Generic;

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
        public DateTime LastSynchronized { get; set; }

        public List<SynchronizationLogEntry> ChangeLog { get; private set; }
    }

    public class SynchronizationLogEntry
    {
        public DateTime TimeStampUtc { get; set; }
    }
}
