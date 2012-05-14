using System;
using System.Web;
using Highway.Shared.Time;

namespace Highway.Shared.Mvc
{
    public class CookieClock : IClock
    {
        public const string ClockCookieName = "SimulatedClockTime";

        public DateTimeOffset SystemTime
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null) return DateTimeOffset.UtcNow;

                var clockCookie = context.Request.Cookies[ClockCookieName];

                if (clockCookie == null) return DateTimeOffset.UtcNow;

                var retval = (DateTimeOffset.FromFileTime(long.Parse(clockCookie.Value))).ToUniversalTime();
                return retval;
            }
            set
            {
                var context = HttpContext.Current;
                if (context == null) return;

                var clockCookie = context.Response.Cookies[ClockCookieName];
                if(clockCookie == null)
                {
                    clockCookie = new HttpCookie(ClockCookieName);
                    context.Response.Cookies.Add(clockCookie);
                }

                clockCookie.Value = value.ToFileTime().ToString();
            }
        }

        public void Reset()
        {
            var context = HttpContext.Current;
            if (context == null) return;

            context.Response.Cookies.Expire(ClockCookieName);
        }
    }
}