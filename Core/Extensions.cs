using System.IO;

namespace ClipEditor.Core
{
    internal static class Extensions
    {
        public static bool HasAnyExtension(this string path, params string[] extensions)
        {
            foreach (string ext in extensions)
            {
                if (Path.GetExtension(path) == ext)
                {
                    return true;
                }
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
