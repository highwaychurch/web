using System;

namespace Highway.Shared.Time
{
    public class SystemClock : IClock
    {
        public DateTimeOffset SystemTime
        {
            get { return DateTimeOffset.UtcNow; }
        }
    }
}