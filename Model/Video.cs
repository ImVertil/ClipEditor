using ClipEditor.Core.Tools;
using System.IO;
using Xabe.FFmpeg;

namespace ClipEditor.Model
{
    internal class Video
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string FileNameWithExtension { get; private set; }
        public string FileExtension { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public IMediaInfo MediaInfo { get; private set; }
        public string Duration => Tools.GetTimeStringFromTimeSpan(TimeSpan);
        public double DurationMs => TimeSpan.TotalMilliseconds;

        public Video(IMediaInfo mediaInfo)
        {
            FilePath = mediaInfo.Path;
            FileName = Path.GetFileNameWithoutExtension(mediaInfo.Path);
            FileNameWithExtension = Path.GetFileName(mediaInfo.Path);
            FileExtension = Path.GetExtension(mediaInfo.Path);
            TimeSpan = mediaInfo.Duration;
            MediaInfo = mediaInfo;
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
