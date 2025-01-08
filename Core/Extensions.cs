namespace ClipEditor.Core
{
    internal static class Extensions
    {
        public static bool EndsWithAny(this string s, params string[] parameters)
        {
            foreach(string parameter in parameters)
            {
                if (s.Contains(parameter))
                    return true;
            }

            return false;
        }

        public static string ToFFmpegRounded(this TimeSpan ts)
        {
            TimeSpan tsRounded = ts.RoundMilliseconds();
            int milliseconds = tsRounded.Milliseconds;
            int seconds = tsRounded.Seconds;
            int minutes = tsRounded.Minutes;
            int hours = (int)tsRounded.TotalHours;

            return $"{hours:D}:{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
        }

        public static TimeSpan RoundMilliseconds(this TimeSpan ts)
        {
            int roundedMs = ts.Milliseconds;
            int roundedS = ts.Seconds;

            switch (ts.Milliseconds)
            {
                case > 750:
                    roundedS += 1;
                    roundedMs = 0;
                    break;

                case > 250:
                    roundedMs = 500;
                    break;

                default:
                    roundedMs = 0;
                    break;
            }

            return new TimeSpan(ts.Days, ts.Hours, ts.Minutes, roundedS, roundedMs);
        }
    }
}
