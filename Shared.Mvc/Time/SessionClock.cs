using System;
using System.Web;
using Highway.Shared.Time;

namespace Highway.Shared.Mvc.Time
{
    public class SessionClock : IClock
    {
        public DateTimeOffset SystemTime
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null) return DateTimeOffset.UtcNow;

                var time = context.Session["SimulatedClockTime"];
                if (time == null) return DateTimeOffset.UtcNow;

                return ((DateTimeOffset)time).ToUniversalTime();
            }
            set
            {
                var context = HttpContext.Current;
                if (context == null) return;

                context.Session["SimulatedClockTime"] = value;
            }
        }

        public void Reset()
        {
            var context = HttpContext.Current;
            if (context == null) return;

            context.Session.Remove("SimulatedClockTime");
        }
    }
}