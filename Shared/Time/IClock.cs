using System;

namespace Highway.Shared.Time
{
    public interface IClock
    {
        /// <summary>
        /// Gets the system time (real or simulated) as UTC.
        /// </summary>
        DateTimeOffset SystemTime { get; }
    }
}
