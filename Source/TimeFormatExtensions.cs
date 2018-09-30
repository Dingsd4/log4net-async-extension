using System;
using System.Text;

namespace log4net
{
    /// <summary>
    /// Provides time format extensions
    /// </summary>
    /// <todo>
    /// Move this extension to its own package
    /// </todo>
    public static class TimeFormatExtensions
    {
        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string FormatTimeShort(this TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + FormatTimeShort(-timeSpan);
            }

            if (timeSpan == TimeSpan.Zero)
            {
                return "0s";
            }

            if (timeSpan.Ticks < TimeSpan.TicksPerMillisecond)
            {
                double nano = (timeSpan.Ticks / (double)(TimeSpan.TicksPerMillisecond / 1000));
                return (nano > 9.99) ? nano.ToString("0.0") + "ns" : nano.ToString("0.00") + "ns";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerSecond)
            {
                double msec = timeSpan.TotalMilliseconds;
                return (msec > 9.99) ? msec.ToString("0.0") + "ms" : msec.ToString("0.00") + "ms";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerMinute)
            {
                double sec = timeSpan.TotalSeconds;
                return (sec > 9.99) ? sec.ToString("0.0") + "s" : sec.ToString("0.00") + "s";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerHour)
            {
                double min = timeSpan.TotalMinutes;
                return (min > 9.99) ? min.ToString("0.0") + "min" : min.ToString("0.00") + "min";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerDay)
            {
                double h = timeSpan.TotalHours;
                return (h > 9.99) ? h.ToString("0.0") + "h" : h.ToString("0.00") + "h";
            }
            double d = timeSpan.TotalDays;
            if (d >= 36525)
            {
                return (d / 365.25).ToString("0") + "a";
            }

            if (d >= 3652.5)
            {
                return (d / 365.25).ToString("0.0") + "a";
            }

            if (d > 99.9)
            {
                return (d / 365.25).ToString("0.00") + "a";
            }

            if (d > 9.99)
            {
                return d.ToString("0.0") + "d";
            }

            return d.ToString("0.00") + "d";
        }

        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatTimeShort(this double seconds)
        {
            if (seconds < 0)
            {
                return "-" + FormatTimeShort(-seconds);
            }

            if (seconds == 0)
            {
                return "0.00";
            }

            if (seconds >= 0.1)
            {
                return FormatTimeShort(TimeSpan.FromTicks((long)(seconds * TimeSpan.TicksPerSecond)));
            }
            double part = seconds;
            for (SiFractions i = SiFractions.m; i <= SiFractions.y; i++)
            {
                part *= 1000.0;
                if (part > 9.99)
                {
                    return part.ToString("0.0") + i + "s";
                }

                if (part > 0.999)
                {
                    return part.ToString("0.00") + i + "s";
                }
            }
            return seconds.ToString() + "s";
        }

        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="milliSeconds"></param>
        /// <returns></returns>
        public static string FormatTimeShort(this long milliSeconds) => FormatTimeShort(milliSeconds / 1000.0);

        /// <summary>Formats the specified timespan to [HH:]MM:SS.F.</summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="millisecondDigits">The number of millisecond digits.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only 0-3 millisecond digits are supported!</exception>
        public static string FormatTimeSpan(this TimeSpan timeSpan, int millisecondDigits)
        {
            StringBuilder result = new StringBuilder();
            if (timeSpan.Hours > 0)
            {
                result.Append(Math.Truncate(timeSpan.TotalHours));
                result.Append(":");
            }
            result.Append(timeSpan.Minutes.ToString("00"));
            result.Append(":");
            int seconds = timeSpan.Seconds;

            switch (millisecondDigits)
            {
                case 0:
                {
                    if (timeSpan.Milliseconds > 0)
                    {
                        seconds++;
                    }

                    result.Append(seconds.ToString("00"));
                    break;
                }
                case 1:
                {
                    result.Append(seconds.ToString("00"));
                    result.Append(".");
                    result.Append(((timeSpan.Milliseconds + 99) / 100).ToString("0"));
                    break;
                }
                case 2:
                {
                    result.Append(seconds.ToString("00"));
                    result.Append(".");
                    result.Append(((timeSpan.Milliseconds + 9) / 10).ToString("00"));
                    break;
                }
                case 3:
                {
                    result.Append(seconds.ToString("00"));
                    result.Append(".");
                    result.Append((timeSpan.Milliseconds).ToString("000"));
                    break;
                }
                default:
                {
                    throw new NotSupportedException("Only 0-3 millisecond digits are supported!");
                }
            }
            return result.ToString();
        }
    }
}
