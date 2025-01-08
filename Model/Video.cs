using ClipEditor.Core.Tools;
using System.IO;

namespace ClipEditor.Model
{
    internal class Video
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string FileNameWithExtension { get; private set; }
        public string FileExtension { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public string Duration => Tools.GetTimeStringFromTimeSpan(TimeSpan);
        public double DurationMs => TimeSpan.TotalMilliseconds;

        public Video(string filePath, TimeSpan duration)
        {
            FilePath = filePath;
            FileName = Path.GetFileNameWithoutExtension(filePath);
            FileNameWithExtension = Path.GetFileName(filePath);
            FileExtension = Path.GetExtension(filePath);
            TimeSpan = duration;
        }

        public Video()
        {
            FilePath = string.Empty;
            FileName = string.Empty;
            FileNameWithExtension = string.Empty;
            FileExtension = string.Empty;
        }
    }
}
