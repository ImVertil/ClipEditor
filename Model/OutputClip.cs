using System.IO;

namespace ClipEditor.Model
{
    internal class OutputClip
    {
        public string FullPath { get; private set; }
        public string Name { get; private set; }
        public string Folder { get; private set; }
        public string ProgressString { get; set; }
        public int Progress { get; set; }
        public ClipStatus Status { get; set; }

        public OutputClip(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            Folder = Path.GetDirectoryName(path) ?? Path.GetFullPath($"out\\");
            ProgressString = "[00:00:00 / 00:00:00]";
            Progress = 0;
            Status = ClipStatus.InProgress;
        }

        public void SetDefaultProperties()
        {
            FullPath = Path.GetFullPath($"out\\defaultVideo");
            Name = string.Empty;
            Folder = Path.GetFullPath($"out\\");
            ProgressString = "[00:00:00 / 00:00:00]";
            Progress = 0;
            Status = ClipStatus.InProgress;
        }
    }

    internal enum ClipStatus
    {
        InProgress,
        AwaitingCancel,
        Canceled,
        Finished
    }
}
