using System;

public static class DateTimeExtensions
{
    public static DateTimeOffset RoundToNearest15Minutes(this DateTimeOffset time)
    {
        return RoundToNearest(time, 15);
    }

    public static DateTimeOffset RoundToNearest5Minutes(this DateTimeOffset time)
    {
        return RoundToNearest(time, 5);
    }

    public static DateTimeOffset RoundToNearest(this DateTimeOffset time, int minutes)
    {
        var mins = time.Minute;
        var intervals = (int)((mins + (minutes / 2)) / minutes);
        mins = (intervals * minutes);

        var result = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, 0, 0, time.Offset);
        return result.AddMinutes(mins);
    }

    public static DateTimeOffset FirstDayOfWeek(this DateTimeOffset time, DayOfWeek startOfWeek)
    {
        var today = time;
        var difference = (int) today.DayOfWeek - (int) startOfWeek;
        if (difference < 0)
            difference += 7;

        return today.Subtract(TimeSpan.FromDays(difference));
    }

    public static string ToPrettyDateString(this DateTimeOffset d, DateTimeOffset? systemTime = null)
    {

        var s = (systemTime == null ? DateTimeOffset.Now : systemTime.Value).Subtract(d);
        var dayDiff = (int)s.TotalDays;
        var secDiff = (int)s.TotalSeconds;
        if(dayDiff < 0 || dayDiff >= 31)
        {
            return null;
        }
        if(dayDiff == 0)
        {
            if(secDiff < 60)
            {
                return "just now";
            }
            if(secDiff < 120)
            {
                return "1 minute ago";
            }
            if(secDiff < 3600)
            {
                return string.Format("{0} minutes ago",
                    Math.Floor((double)secDiff / 60));
            }
            if(secDiff < 7200)
            {
                return "1 hour ago";
            }
            if(secDiff < 86400)
            {
                return string.Format("{0} hours ago",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        if(dayDiff == 1)
        {
            return "yesterday";
        }
        if(dayDiff < 7)
        {
            return string.Format("{0} days ago",
            dayDiff);
        }
        if(dayDiff < 31)
        {
            return string.Format("{0} weeks ago",
            Math.Ceiling((double)dayDiff / 7));
        }
        return null;
    }

    public static TimeSpan RoundToNearest(this TimeSpan time, int minutes)
    {
        var mins = time.TotalMinutes;
        var intervals = (int)((mins + (minutes / 2)) / minutes);
        mins = (intervals * minutes);

        return TimeSpan.FromMinutes(mins);
    }
}
